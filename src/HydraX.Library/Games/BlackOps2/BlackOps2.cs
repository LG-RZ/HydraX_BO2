using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HydraX.Library
{
    public partial class BlackOps2 : IGame
    {
        /// <summary>
        /// Alias Hashes and Names
        /// </summary>
        public static Dictionary<uint, string> AliasHashes = new Dictionary<uint, string>();

        /// <summary>
        /// Gets Black Ops 2's Game Name
        /// </summary>
        public string Name => "Black Ops II";

        /// <summary>
        /// Gets Black Ops 2's Process Names
        /// </summary>
        public string[] ProcessNames => new string[]
        {
            "t6zm",
            "t6mp",
            "t6sp"
        };

        /// <summary>
        /// Gets Black Ops 2 Asset Pools Addresses
        /// </summary>
        public long AssetPoolsAddress { get; set; }

        /// <summary>
        /// Gets Black Ops 2 Asset Sizes Addresses
        /// </summary>
        public long AssetSizesAddress { get; set; }

        /// <summary>
        /// Gets Black Ops 2 String Pool
        /// </summary>
        public long StringPoolAddress { get; set; }

        /// <summary>
        /// Gets or Sets Black Ops 2's Base Address (ASLR)
        /// </summary>
        public long BaseAddress { get; set; }

        /// <summary>
        /// Gets or Sets the current Process Index
        /// </summary>
        public int ProcessIndex { get; set; }

        /// <summary>
        /// Gets the DBAssetSize
        /// </summary>
        public byte DBAssetSize => 4;

        /// <summary>
        /// Gets or sets the list of Asset Pools
        /// </summary>
        public List<IAssetPool> AssetPools { get; set; }

        /// <summary>
        /// Gets or Sets the Zone Names for each asset
        /// </summary>
        public Dictionary<long, string> ZoneNames { get; set; }

        /// <summary>
        /// Black Ops II Asset Pool Indices
        /// </summary>
        private enum AssetPool : int
        {
            xmodelpieces,
            physpreset,
            physconstraints,
            destructibledef,
            xanimparts,
            xmodel,
            material,
            techset,
            image,
            sound,
            sound_patch,
            clipmap,
            clipmap_pvs,
            comworld,
            gameworld_sp,
            gameworld_mp,
            map_ents,
            gfxworld,
            light_def,
            ui_map,
            font,
            fonticon,
            menulist,
            menu,
            localize,
            weapon,
            weapondef,
            weapon_variant,
            weapon_full,
            attachment,
            attachment_unique,
            weapon_camo,
            snddriver_globals,
            fx,
            impact_fx,
            aitype,
            mptype,
            mpbody,
            mphead,
            character,
            xmodelalias,
            rawfile,
            stringtable,
            leaderboard,
            xglobals,
            ddl,
            glasses,
            emblemset,
            scriptparsetree,
            keyvaluepairs,
            vehicledef,
            memoryblock,
            addon_map_ents,
            tracer,
            skinnedverts,
            qdb,
            slug,
            footstep_table,
            footstepfx_table,
            zbarrier,
            count,
            _string,
            assetlist,
            report,
            depend,
            full_count
        }

        /// <summary>
        /// Gets an Alias Name by Hash, if not found, returns the hash as a hex string
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string GetAliasByHash(uint hash)
        {
            if (hash == 0)
                return "";

            return AliasHashes.TryGetValue(hash, out var alias) ? alias : "";
        }

        public string CleanAssetName(HydraAssetType type, string assetName)
        {
            if (!string.IsNullOrWhiteSpace(assetName))
            {
                switch (type)
                {
                    case HydraAssetType.FX:
                        return Path.ChangeExtension(assetName.Replace(@"/", @"\").Replace(@"\", @"\\"), ".efx");
                    case HydraAssetType.Sound:
                        return Path.ChangeExtension(assetName.Split('.')[0].Replace("raw\\sound\\", ""), ".wav");
                    default:
                        return assetName;
                }
            }

            return "";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string GetAssetName(long address, HydraInstance instance, long offset = 0)
        {
            return address == 0 ? "" : instance.Reader.ReadNullTerminatedString(instance.Reader.ReadUInt32(address + offset));
        }

        public string GetString(long index, HydraInstance instance)
        {
            return instance.Reader.ReadNullTerminatedString(instance.Game.StringPoolAddress + (18 * index) + 4);
        }

        public bool Initialize(HydraInstance instance)
        {
            var module = instance.Reader.Modules[0];

            var pools = instance.Reader.FindBytes(
                new byte?[] { 0x56, 0x51, 0xFF, 0xD2, 0x8B, 0xF0, 0x83, 0xC4, 0x04, 0x85, 0xF6 },
                module.BaseAddress.ToInt64(),
                module.BaseAddress.ToInt64() + module.Size,
                true);
            var strPool = instance.Reader.FindBytes(
                new byte?[] { 0x33, 0xC9, 0x66, 0x89, 0x0C, 0x10, 0x83, 0xC0, 0x02, 0x83, 0xF8, 0x20 },
                module.BaseAddress.ToInt64(),
                module.BaseAddress.ToInt64() + module.Size,
                true);

            if(pools.Length > 0 && strPool.Length > 0)
            {
                AssetPoolsAddress = instance.Reader.ReadInt32(pools[0] - 0xB);
                AssetSizesAddress = instance.Reader.ReadInt32(pools[0] + 0x3B);
                StringPoolAddress = instance.Reader.ReadInt32(strPool[0] - 0x2E);
                
                string FirstXModel = instance.Reader.ReadNullTerminatedString(instance.Reader.ReadUInt32(instance.Reader.ReadUInt32(AssetPoolsAddress + (DBAssetSize * (int)AssetPool.xmodel)) + DBAssetSize));

                if(FirstXModel == "defaultvehicle")
                {
                    if(!string.IsNullOrWhiteSpace(GetString(2, instance)))
                    {                  
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Converts an asset buffer to GDT Asset, the buffer passed will be the exact same as the buffer Linker uses to load the GDT asset
        /// </summary>
        public static GameDataTable.Asset ConvertAssetBufferToGDTAsset(byte[] assetBuffer, Tuple<string, int, int>[] properties, HydraInstance instance, Func<GameDataTable.Asset, byte[], int, int, HydraInstance, object> extendedDataHandler = null)
        {
            var asset = new GameDataTable.Asset();

            foreach (var property in properties)
            {
                switch (property.Item3)
                {
                    // Strings (Enum that points to a string)
                    case 0:
                        {
                            asset[property.Item1] = instance.Reader.ReadNullTerminatedString(BitConverter.ToUInt32(assetBuffer, property.Item2));
                            break;
                        }
                    // Inline Character Array
                    case 1:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 1024).TrimEnd('\0');
                            break;
                        }
                    // Inline Character Array
                    case 2:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 64).TrimEnd('\0');
                            break;
                        }
                    // Inline Character Array
                    case 3:
                        {
                            asset[property.Item1] = Encoding.ASCII.GetString(assetBuffer, property.Item2, 256).TrimEnd('\0');
                            break;
                        }
                    // 32Bit Ints
                    case 4:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Unsigned Ints
                    case 5:
                        {
                            asset[property.Item1] = BitConverter.ToUInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Bools
                    case 6:
                        {
                            asset[property.Item1] = assetBuffer[property.Item2];
                            break;
                        }
                    // QBools
                    case 7:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2);
                            break;
                        }
                    // Floats
                    case 8:
                        {
                            asset[property.Item1] = BitConverter.ToSingle(assetBuffer, property.Item2);
                            break;
                        }
                    // Milliseconds
                    case 9:
                        {
                            asset[property.Item1] = BitConverter.ToInt32(assetBuffer, property.Item2) / 1000.0;
                            break;
                        }
                    // Script Strings
                    case 0x15:
                        {
                            asset[property.Item1] = instance.Game.GetString(BitConverter.ToInt32(assetBuffer, property.Item2), instance);
                            break;
                        }
                    case 0xA:
                        {
                            var assetName = instance.Game.GetAssetName(BitConverter.ToUInt32(assetBuffer, property.Item2), instance, 0);
                            asset[property.Item1] = string.IsNullOrWhiteSpace(assetName) ? "" : @"fx\\" + Path.ChangeExtension(assetName.Replace(@"/", @"\").Replace(@"\", @"\\"), ".efx");
                            break;
                        }
                    // Images
                    case 0x10:
                    case 0x11:
                        {
                            asset[property.Item1] = instance.Game.GetAssetName(BitConverter.ToUInt32(assetBuffer, property.Item2), instance, 0xF8);
                            break;
                        }
                    // Alias Hash
                    case 0x18:
                        {
                            asset[property.Item1] = "";//GetAliasByHash(BitConverter.ToUInt32(assetBuffer, property.Item2));
                            break;
                        }
                    // Flametable Assets
                    case 0x2B:
                        {
                            asset[property.Item1] = instance.Game.GetAssetName(BitConverter.ToUInt32(assetBuffer, property.Item2), instance, 0x1B0);
                            break;
                        }
                    // Standard Assets
                    case 0xB:
                    case 0xD:
                    case 0xF:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x16:
                    case 0x17:
                    case 0x19:
                    case 0x1A:
                    case 0x1B:
                    case 0x1C:
                    case 0x1D:
                    case 0x1E:
                    case 0x1F:
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 0x28:
                    case 0x29:
                    case 0x2A:
                    case 0x2C:
                    case 0x2D:
                    case 0x2E:
                    case 0x2F:
                    case 0x30:
                    case 0x31:
                    case 0x32:
                        {
                            var assetName = instance.Game.GetAssetName(BitConverter.ToUInt32(assetBuffer, property.Item2), instance, property.Item3 == 0x10 ? 0xF8 : 0);
                            asset[property.Item1] = assetName;
                            break;
                        }
                    default:
                        {
                            // Attempt to use the extended data handler, otherwise null
                            var result = extendedDataHandler?.Invoke(asset, assetBuffer, property.Item2, property.Item3, instance);

                            if (result != null)
                            {
                                asset[property.Item1] = result;
                            }
                            else
                            {
                                asset[property.Item1] = "";
#if DEBUG
                                Console.WriteLine("Unknown Value: {0} - {1} - {2}", property.Item3, property.Item2, property.Item1);
#endif
                            }
                            // Done
                            break;
                        }
                }

                //assetBuffer[property.Item2] = 0xAF;
            }

            // Done
            return asset;
        }
    }
}
