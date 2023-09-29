using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Windows;
using HydraX.Library.AssetContainers;
using PhilLibX;
using System.Text;
using System.Linq;

namespace HydraX.Library
{
    using Pointer = UInt32;
    public partial class BlackOps2
    {
        private class Material : IAssetPool
        {
            #region AssetStructures

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x10)]
            private struct vec4_t
            {
                public vec4_t(float X, float Y, float Z, float W)
                {
                    this.X = X;
                    this.Y = Y;
                    this.Z = Z;
                    this.W = W;
                }

                public float X;
                public float Y;
                public float Z;
                public float W;

                public static readonly vec4_t One = new vec4_t(1f, 1f, 1f, 1f);

                public override string ToString()
                {
                    return string.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x30)]
            private unsafe struct MaterialInfo
            {
                public Pointer name; // Char *
                public uint gameFlags;
                public byte pad;
                public byte sortKey;
                public byte textureAtlasRowCount;
                public byte textureAtlasColumnCount;
                public fixed byte padding[4];
                public long drawSurf; // GfxDrawSurf
                public uint surfaceTypeBits;
                public uint layeredSurfaceTypes;
                public ushort hashIndex;
                public fixed byte padding2[2];
                public int surfaceFlags;
                public int contents;
                public fixed byte padding3[4];
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x10)]
            private unsafe struct MaterialTextureDef
            {
                public uint nameHash;
                public char nameStart;
                public char nameEnd;
                public byte samplerState;
                public byte semantic;
                public byte isMatureContent;
                public fixed byte pad[3];
                public Pointer image; // GfxImage *
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0x50)]
            private unsafe struct GfxImage
            {
                public uint texture;
                public byte mapType;
                public byte semantic;
                public byte category;
                public byte delayLoadPixels;
                public ushort picmip;
                public byte noPicmip;
                public byte track;
                public long cardMemory;
                public ushort width;
                public ushort height;
                public ushort depth;
                public byte levelCount;
                public byte streaming;
                public uint baseSize;
                public Pointer pixels; // uchar *
                public fixed byte streamedParts[0x18]; // GfxStreamedPartInfo *
                public byte streamedPartCount;
                public uint loadedSize;
                public byte skippedMipLevels;
                public Pointer name; // char *
                public uint hash;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0x20)]
            private unsafe struct MaterialConstantDef
            {
                public uint hash;
                public fixed byte name[12];
                public vec4_t literal;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0x70)]
            private unsafe struct MaterialAsset
            {
                public MaterialInfo info;
                public fixed byte stateBitsEntry[36];
                public byte textureCount;
                public byte constantCount;
                public byte stateBitsCount;
                public byte stateFlags;
                public byte cameraRegion;
                public byte probeMipBits;
                public Pointer techniqueSet; // MaterialTechniqueSet *
                public Pointer textureTable; // MaterialTextureDef *
                public Pointer constantTable; // MaterialConstantDef *
                public Pointer stateBitsTable; // GfxStateBits *
                public Pointer thermalMaterial; // MaterialAsset *
            }

            private readonly static string[] imageTypeName =
            {
                "misc",
                "debug",
                "tex",
                "ui",
                "lmap",
                "light",
                "f/x",
                "hud",
                "model",
                "world",
            };

            public static Dictionary<uint, string> SurfaceTypesr = new Dictionary<uint, string>()
            {
                { 23068672, "asphalt" },
                { 1048576 , "bark" },
                { 2097152, "brick" },
                { 3145728, "carpet" },
                { 4194304, "cloth" },
                { 5242880, "concrete" },
                { 6291456, "dirt" },
                { 7340032, "flesh" },
                { 8388608, "foliage" },
                { 9437184, "glass" },
                { 10485760, "grass" },
                { 11534336,"gravel"  },
                { 12582912, "ice" },
                { 13631488, "metal" },
                { 14680064, "mud" },
                { 15728640, "paper" },
                { 16777216, "plaster" },
                { 17825792, "rock" },
                { 18874368, "sand" },
                { 19922944, "snow" },
                { 20971520, "water"  },
                { 22020096, "wood" },
                { 24117248, "ceramic" },
                { 25165824, "plastic" },
                { 26214400, "rubber" },
                { 27262976, "cushion"  },
                { 28311552, "fruit" },
                { 29360128, "paintedmetal" },
                { 31457280, "tallgrass" },
                { 32505856, "riotshield" },
                { 30408704, "player" },
                { 33554432, "metalthin" },
                { 34603008, "metalhollow" },
                { 35651584, "metalcatwalk" },
                { 36700160, "metalcar" },
                { 37748736, "glasscar" },
                { 38797312, "glassbulletproof" },
                { 39845888, "watershallow" },
                { 40894464, "bodyarmor" }
            };

            public static Dictionary<(uint, uint), string> SurfaceTypes = new Dictionary<(uint, uint), string>()
            {
                [(0x100000, 0x0)] = "bark",
                [(0x200000, 0x0)] = "brick",
                [(0x300000, 0x0)] = "carpet",
                [(0x400000, 0x0)] = "cloth",
                [(0x500000, 0x0)] = "concrete",
                [(0x600000, 0x0)] = "dirt",
                [(0x700000, 0x0)] = "flesh",
                [(0x800000, 0x2)] = "foliage",
                [(0x900000, 0x10)] = "glass",
                [(0xA00000, 0x0)] = "grass",
                [(0xB00000, 0x0)] = "gravel",
                [(0xC00000, 0x0)] = "ice",
                [(0xD00000, 0x0)] = "metal",
                [(0xE00000, 0x0)] = "mud",
                [(0xF00000, 0x0)] = "paper",
                [(0x1000000, 0x0)] = "plaster",
                [(0x1100000, 0x0)] = "rock",
                [(0x1200000, 0x0)] = "sand",
                [(0x1300000, 0x0)] = "snow",
                [(0x1400000, 0x20)] = "water",
                [(0x1500000, 0x0)] = "wood",
                [(0x1600000, 0x0)] = "asphalt",
                [(0x1700000, 0x0)] = "ceramic",
                [(0x1800000, 0x0)] = "plastic",
                [(0x1900000, 0x0)] = "rubber",
                [(0x1A00000, 0x0)] = "cushion",
                [(0x1B00000, 0x0)] = "fruit",
                [(0x1C00000, 0x0)] = "paintedmetal",
                [(0x1D00000, 0x0)] = "player",
                [(0x1E00000, 0x0)] = "tallgrass",
                [(0x1F00000, 0x0)] = "riotshield",
                [(0x900000, 0x0)] = "opaqueglass",
                [(0x0, 0x80)] = "clipmissle",
                [(0x0, 0x1000)] = "ai_nosight",
                [(0x0, 0x2000)] = "clipshot",
                [(0x0, 0x10000)] = "playerclip",
                [(0x0, 0x20000)] = "monsterclip",
                [(0x0, 0x200)] = "vehicleclip",
                [(0x0, 0x400)] = "itemclip",
                [(0x0, 0x80000000)] = "noDrop",
                [(0x4000, 0x0)] = "nonSolid",
                [(0x0, 0x8000000)] = "detail",
                [(0x0, 0x10000000)] = "structural",
                [(0x80000000, 0x0)] = "portal",
                [(0x0, 0x40)] = "canShootClip",
                //[(0x0, 0x0, 0x0)] = "origin",
                [(0x4, 0x800)] = "sky",
                [(0x40000, 0x0)] = "noCastShadow",
                [(0x80000, 0x0)] = "onlyCastShadow",
                //[(0x0, 0x0, 0x400)] = "physicsGeom",
                //[(0x0, 0x0, 0x2000)] = "lightPortal",
                [(0x1000, 0x0)] = "caulk",
                [(0x8000, 0x0)] = "areaLight",
                [(0x2, 0x0)] = "slick",
                [(0x10, 0x0)] = "noImpact",
                [(0x20, 0x0)] = "noMarks",
                [(0x100, 0x0)] = "noPenetrate",
                [(0x8, 0x0)] = "ladder",
                [(0x1, 0x0)] = "noDamage",
                [(0x4000000, 0x1000000)] = "mantleOn",
                [(0x8000000, 0x1000000)] = "mantleOver",
                [(0x10000000, 0x1000000)] = "mount",
                [(0x2000, 0x0)] = "noSteps",
                [(0x80, 0x0)] = "noDraw",
                [(0x800, 0x0)] = "noReceiveDynamicShadow",
                [(0x20000, 0x0)] = "noDlight",
            };

            #endregion

            public string Name => "material";

            public string SettingGroup => "material";

            public int Index => (int)AssetPool.material;

            public int AssetSize { get; set; }
            public int AssetCount { get; set; }
            public long StartAddress { get; set; }
            public long EndAddress { get { return StartAddress + (AssetCount * AssetSize); } set => throw new NotImplementedException(); }

            public List<Asset> Load(HydraInstance instance)
            {
                List<Asset> results = new List<Asset>();

                StartAddress = instance.Reader.ReadStruct<uint>(instance.Game.AssetPoolsAddress + (Index * 4)) + 0x8;
                AssetCount = instance.Reader.ReadStruct<int>(instance.Game.AssetSizesAddress + (Index * 4));
                AssetSize = Marshal.SizeOf<MaterialAsset>();

                for (int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<MaterialAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.info.name))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.info.name),
                        Type = Name,
                        Status = "Loaded",
                        Data = address,
                        LoadMethod = ExportAsset,
                        Zone = "none",
                        Information = "N/A"
                    });
                }

                return results;
            }

            public unsafe void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<MaterialAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.info.name))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                string path = Path.Combine(instance.ExportFolder, "materials", asset.Name + ".bo2mat");
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using(StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine("Material : ()");
                    writer.WriteLine("{");

                    writer.WriteLine("\tName : \"{0}\"", asset.Name);
                    writer.WriteLine("\tTechnique Set : \"{0}\"", instance.Game.GetAssetName(header.techniqueSet, instance));
                    writer.WriteLine("\tTextureAtlasRowCount : {0}", header.info.textureAtlasRowCount);
                    writer.WriteLine("\tTextureAtlasColumnCount : {0}", header.info.textureAtlasColumnCount);

                    if (header.textureCount != 0)
                    {
                        writer.WriteLine();
                        writer.WriteLine("\tMaterialTextureDef[{0}] : ()", header.textureCount);
                        writer.WriteLine("\t{");
                        for(int i = 0; i < header.textureCount; i++)
                        {
                            MaterialTextureDef materialTextureDef = instance.Reader.ReadStruct<MaterialTextureDef>(header.textureTable + (Marshal.SizeOf<MaterialTextureDef>() * i));
                            GfxImage gfxImage = instance.Reader.ReadStruct<GfxImage>(materialTextureDef.image);

                            writer.WriteLine("\t\tGfxImage : ()");
                            writer.WriteLine("\t\t{");
                            writer.WriteLine("\t\t\tName: \"{0}\"", instance.Reader.ReadNullTerminatedString(gfxImage.name));
                            File.AppendAllText(Path.Combine(instance.ExportFolder, "materials", "needed_textures.txt"), string.Format("{0}, ", instance.Reader.ReadNullTerminatedString(gfxImage.name)));
                            writer.WriteLine("\t\t\tImageType: {0}", imageTypeName[gfxImage.semantic]);
                            writer.WriteLine("\t\t\tWidth: {0}", gfxImage.width);
                            writer.WriteLine("\t\t\tHeight: {0}", gfxImage.height);
                            writer.WriteLine("\t\t\tDepth: {0}", gfxImage.depth);
                            writer.WriteLine("\t\t\tLevelCount: {0}", gfxImage.levelCount);
                            writer.WriteLine("\t\t}");

                            if (i != (header.textureCount - 1))
                                writer.WriteLine();
                        }
                        writer.WriteLine("\t}");
                    }

                    uint colorTintHash = R_HashString("colorTint", 0);
                    bool hasColorTint = false;
                    for (int i = 0; i < header.constantCount; i++)
                    {
                        MaterialConstantDef materialConstantDef = instance.Reader.ReadStruct<MaterialConstantDef>(header.constantTable + (Marshal.SizeOf<MaterialConstantDef>() * i));

                        if (materialConstantDef.hash == colorTintHash)
                        {
                            hasColorTint = true;
                            break;
                        }
                    }

                    if (header.constantCount != 0)
                    {
                        writer.WriteLine();
                        writer.WriteLine("\tMaterialConstantDef[{0}] : ()", header.constantCount + Convert.ToInt32(!hasColorTint));
                        writer.WriteLine("\t{");

                        if(!hasColorTint)
                        {
                            writer.WriteLine("\t\tMaterialConstantDef : ()");
                            writer.WriteLine("\t\t{");
                            writer.WriteLine("\t\t\tHash: 0x{0:X}", colorTintHash);
                            writer.WriteLine("\t\t\tName: \"{0}\"", "colorTint");
                            writer.WriteLine("\t\t\tLiteral: {0}", vec4_t.One);
                            writer.WriteLine("\t\t}");
                            writer.WriteLine();
                        }

                        for (int i = 0; i < header.constantCount; i++)
                        {
                            MaterialConstantDef materialConstantDef = instance.Reader.ReadStruct<MaterialConstantDef>(header.constantTable + (Marshal.SizeOf<MaterialConstantDef>() * i));

                            writer.WriteLine("\t\tMaterialConstantDef : ()");
                            writer.WriteLine("\t\t{");
                            writer.WriteLine("\t\t\tHash: 0x{0:X}", materialConstantDef.hash);
                            writer.WriteLine("\t\t\tName: \"{0}\"", Encoding.UTF8.GetString(materialConstantDef.name, 12).TrimEnd('\0'));
                            writer.WriteLine("\t\t\tLiteral: {0}", materialConstantDef.literal);
                            writer.WriteLine("\t\t}");

                            if(i != (header.constantCount - 1))
                                writer.WriteLine();
                        }
                        writer.WriteLine("\t}");
                    }
                    else
                    {
                        writer.WriteLine();
                        writer.WriteLine("\tMaterialConstantDef[1] : ()");
                        writer.WriteLine("\t{");

                        writer.WriteLine("\t\tMaterialConstantDef : ()");
                        writer.WriteLine("\t\t{");
                        writer.WriteLine("\t\t\tHash: 0x{0:X}", colorTintHash);
                        writer.WriteLine("\t\t\tName: \"{0}\"", "colorTint");
                        writer.WriteLine("\t\t\tLiteral: {0}", vec4_t.One);
                        writer.WriteLine("\t\t}");

                        writer.WriteLine("\t}");
                    }

                    writer.WriteLine("}");
                }
            }

            public bool IsNullAsset(long nameAddress)
            {
                return nameAddress >= StartAddress && nameAddress <= EndAddress || nameAddress == 0;
            }

            public static uint R_HashString(string value, uint hash)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    hash = (uint)((int)value[i] | 0x20U) ^ hash * 0x21;
                }
                return hash;
            }
        }
    }
}
