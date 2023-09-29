using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Windows;
using HydraX.Library.AssetContainers;
using PhilLibX;
using System.Text;

namespace HydraX.Library
{
    using pointer = UInt32;
    public partial class BlackOps2
    {
        private class Sound : IAssetPool
        {
            private enum SndBankState : int
            {
                SND_BANK_STATE_ERROR = 8,
                SND_BANK_STATE_LOADED_ASSETS = 6,
                SND_BANK_STATE_LOADED_ASSET_WAIT = 5,
                SND_BANK_STATE_LOADED_HEADER = 3,
                SND_BANK_STATE_LOADED_TOC = 4,
                SND_BANK_STATE_NEW = 0,
                SND_BANK_STATE_READY_TO_USE = 7,
                SND_BANK_STATE_STREAM_HEADER = 1,
                SND_BANK_STATE_STREAM_TOC = 2
            }

            #region AssetStructures
            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x1294)]
            private unsafe struct SndBank
            {
                #region SoundAssetProperties
                public pointer name; // char*
                public uint aliasCount;
                public pointer alias; // SndAliasList*
                public pointer aliasIndex; // SndIndexEntry *
                public uint radverbCount;
                public pointer radverbs; // SndRadverb *
                public uint duckCount;
                public pointer ducks; // SndDuck *
                public SndRuntimeAssetBank streamAssetBank;
                public SndRuntimeAssetBank loadAssetBank;
                public SndLoadedAssets loadedAssets;
                public uint scriptIdLookupCount;
                public pointer scriptIdLookups; // SndDialogScriptIdLookup
                public SndBankState state;
                public int streamRequestId;
                [MarshalAs(UnmanagedType.U1)]
                public bool pendingIo;
                [MarshalAs(UnmanagedType.U1)]
                public bool ioError;
                [MarshalAs(UnmanagedType.U1)]
                public bool runtimeAssetLoad;
                #endregion
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x8)]
            private struct SndDialogScriptIdLookup
            {
                public uint scriptId;
                public uint aliasId;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x14)]
            private struct SndAssetBankEntry
            {
                public uint id;
                public uint size;
                public uint offset;
                public uint frameCount;
                public byte frameRateIndex;
                public byte channelCount;
                public byte looping;
                public byte format;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x1c)]
            private struct SndLoadedAssets
            {
                public pointer zone; // char *
                public pointer language; // char *
                public uint loadedCount;
                public uint entryCount;
                public pointer entries; // SndAssetBankEntry *
                public uint dataSize;
                public pointer data; // char *
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x800)]
            private unsafe struct SndAssetBankHeader
            {
                public uint magic;
                public uint version;
                public uint entrySize;
                public uint checksumSize;
                public uint dependencySize;
                public uint entryCount;
                public uint dependencyCount;
                public uint pad32;
                public long fileSize;
                public long entryOffset;
                public long checksumOffset;
                public fixed byte checksumChecksum[16];
                public fixed byte dependencies[512];  // fixed char[]
                public fixed byte padding[1464];  // fixed char[]
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x922)]
            private unsafe struct SndRuntimeAssetBank
            {
                public pointer zone; // char *
                public pointer language; // char *
                public int fileHandle;
                public SndAssetBankHeader header;
                public uint entryOffset;
                public fixed byte linkTimeChecksum[16];
                public fixed byte filename[256]; // fixed char[]
                [MarshalAs(UnmanagedType.U1)]
                public bool indicesLoaded;
                [MarshalAs(UnmanagedType.U1)]
                public bool indicesAllocated;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x4)]
            private struct SndIndexEntry
            {
                public ushort value;
                public ushort next;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x64)]
            private unsafe struct SndRadverb
            {
                public fixed char name[32]; // fixed char[]
                public uint id;
                public float smoothing;
                public float earlyTime;
                public float lateTime;
                public float earlyGain;
                public float lateGain;
                public float returnGain;
                public float earlyLpf;
                public float lateLpf;
                public float inputLpf;
                public float dampLpf;
                public float wallReflect;
                public float dryGain;
                public float earlySize;
                public float lateSize;
                public float diffusion;
                public float returnHighpass;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x4C)]
            private unsafe struct SndDuck
            {
                public fixed byte name[32]; // fixed char[]
                public uint id;
                public float fadeIn;
                public float fadeOut;
                public float startDelay;
                public float distance;
                public float length;
                public uint fadeInCurve;
                public uint fadeOutCurve;
                public pointer attenuation; // float *
                public pointer filter; // float *
                public int updateWhilePaused;
            };

            #region Alias

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x14)]
            private struct SndAliasList
            {
                public pointer name;
                public uint id;
                public pointer head;
                public int count;
                public int sequence;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x60)]
            private struct SndAlias
            {
                public pointer name;
                public uint id;
                public pointer subtitle;
                public pointer secondaryname;
                public uint assetId;
                public pointer assetFileName;
                public uint flags0;
                public uint flags1;
                public uint duck;
                public uint contextType;
                public uint contextValue;
                public uint stopOnPlay;
                public uint futzPatch;
                public ushort fluxTime;
                public ushort startDelay;
                public ushort reverbSend;
                public ushort centerSend;
                public ushort volMin;
                public ushort volMax;
                public ushort pitchMin;
                public ushort pitchMax;
                public ushort distMin;
                public ushort distMax;
                public ushort distReverbMax;
                public ushort envelopMin;
                public ushort envelopMax;
                public ushort envelopPercentage;
                public short fadeIn;
                public short fadeOut;
                public short dopplerScale;
                public byte minPriorityThreshold;
                public byte maxPriorityThreshold;
                public byte probability;
                public byte occlusionLevel;
                public byte minPriority;
                public byte maxPriority;
                public byte pan;
                public byte limitCount;
                public byte entityLimitCount;
                public byte duckGroup;
            };

            #endregion

            #endregion

            #region Helpers

            private static readonly string[] GROUPS =
            {
                "grp_wpn_lfe",
                "grp_lfe",
                "grp_hdrfx",
                "grp_music",
                "grp_voice",
                "grp_set_piece",
                "grp_igc",
                "grp_mp_game",
                "grp_explosion",
                "grp_player_impacts",
                "grp_scripted_moment",
                "grp_menu",
                "grp_whizby",
                "grp_weapon",
                "grp_vehicle",
                "grp_impacts",
                "grp_foley",
                "grp_destructible",
                "grp_physics",
                "grp_ambience",
                "grp_alerts",
                "grp_air",
                "grp_bink",
                "grp_announcer",
            };

            private static readonly string[] CURVES =
            {
                "default",
                "defaultmin",
                "allon",
                "alloff",
                "rcurve0",
                "rcurve1",
                "rcurve2",
                "rcurve3",
                "rcurve4",
                "rcurve5",
                "steep",
                "sindelay",
                "cosdelay",
                "sin",
                "cos",
                "rev60",
                "rev65",
            };

            private static string[] RESTRICT_IDS =
            {
                "unrestricted",
                "restricted",
                "",
            };

            private static string[] PAUSE_IDS =
            {
                "nopause",
                "pause",
                "",
            };

            private static string[] LIMIT_IDS =
            {
                "none",
                "oldest",
                "reject",
                "softest",
                "",
            };

            private static string[] CATEGORY_IDS =
            {
                "sfx",
                "music",
                "voice",
                "ui",
                "",
            };

            private static string[] BUS_IDS =
            {
                "bus_reverb",
                "bus_fx",
                "bus_voice",
                "bus_pfutz",
                "bus_hdrfx",
                "bus_ui",
                "bus_music",
                "bus_movie",
                "bus_reference",
                "",
            };

            private static string[] LOAD_TYPES =
            {
                "unknown",
                "loaded",
                "streamed",
                "primed",
                "",
            };

            private static string[] LIMIT_TYPES =
            {
                "none",
                "oldest",
                "reject",
                "priority",
                "",
            };

            private static string[] MOVE_TYPES =
            {
                "none",
                "left_player",
                "center_player",
                "right_player",
                "random_player",
                "left_shot",
                "center_shot",
                "right_shot",
                "",
            };

            private static string[] RANDOMIZE_TYPES =
            {
                "volume",
                "pitch",
                "variant",
                "",
            };

            private static string[] PAN_TYPES =
            {
                "2d",
                "3d",
                "",
            };

            private static string[] MATURE_TYPES =
            {
                "both",
                "yes",
                "no",
                "",
            };

            private static string[] LOOP_TYPES =
            {
                "nonlooping",
                "looping",
                "",
            };

            private static string[] NO_YES =
            {
                "False",
                "True",
                "",
            };

            private static string[] OUTPUT_DEVICES =
            {
                "tv",
                "drc",
                "rmt0",
                "rmt1",
                "rmt2",
                "rmt3",
                "",
            };

            private static readonly string[] PANS =
            {
                "default",
                "music",
                "wpn_all",
                "wpn_fnt",
                "wpn_rear",
                "wpn_left",
                "wpn_right",
                "music_all",
                "fly_foot_all",
                "front",
                "back",
                "front_mostly",
                "back_mostly",
                "all",
                "center",
                "front_and_center",
                "lfe",
                "quad",
                "front_mostly_some_center",
                "front_halfback",
                "halffront_back",
                "test",
                "brass_right",
                "brass_left",
                "veh_back",
                "tst_left",
                "tst_center",
                "tst_right",
                "tst_surround_left",
                "tst_surround_right",
                "tst_lfe",
                "pip",
                "movie_vo",
            };

            private static readonly string[] DUCK_GROUPS =
            {
                "snp_alerts_gameplay",
                "snp_ambience",
                "snp_claw",
                "snp_destructible",
                "snp_dying",
                "snp_dying_ice",
                "snp_evt_2d",
                "snp_explosion",
                "snp_foley",
                "snp_grenade",
                "snp_hdrfx",
                "snp_igc",
                "snp_impacts",
                "snp_menu",
                "snp_movie",
                "snp_music",
                "snp_never_duck",
                "snp_player_dead",
                "snp_player_impacts",
                "snp_scripted_moment",
                "snp_set_piece",
                "snp_special",
                "snp_vehicle",
                "snp_vehicle_interior",
                "snp_voice",
                "snp_weapon_decay_1p",
                "snp_whizby",
                "snp_wpn_1p",
                "snp_wpn_3p",
                "snp_wpn_turret",
                "snp_x2",
                "snp_x3",
            };

            public static int CalculatePitch(ushort input)
            {
                return (int)Math.Ceiling(Math.Log(input / 32767.0, 2.0) * 1200);
            }

            public static double Calculate100Value(ushort input)
            {
                return input > 0 ? Math.Round((Math.Log(input / 65535f, 10.0) / 0.05) + 100.0, 2) : 0;
            }

            #endregion

            public string Name => "sound";

            public string SettingGroup => "sound";

            public int Index => (int)AssetPool.sound;

            public int AssetSize { get; set; }
            public int AssetCount { get; set; }
            public long StartAddress { get; set; }
            public long EndAddress { get { return StartAddress + (AssetCount * AssetSize); } set => throw new NotImplementedException(); }

            public List<Asset> Load(HydraInstance instance)
            {
                List<Asset> results = new List<Asset>();

                StartAddress = instance.Reader.ReadStruct<uint>(instance.Game.AssetPoolsAddress + (Index * 4));
                AssetCount = instance.Reader.ReadStruct<int>(instance.Game.AssetSizesAddress + (Index * 4));
                AssetSize = Marshal.SizeOf<SndBank>();

                for (int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<SndBank>(StartAddress + (i * AssetSize) + 4);

                    if (IsNullAsset(header.name))
                        continue;

                    string zone = "unknown";

                    if (!IsNullAsset(header.streamAssetBank.zone))
                        zone = instance.Reader.ReadNullTerminatedString(header.streamAssetBank.zone);

                    var address = StartAddress + (i * AssetSize) + 4;

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.name),
                        Type = Name,
                        Status = "Loaded",
                        Data = address,
                        LoadMethod = ExportAsset,
                        Zone = zone,
                        Information = string.Format("Aliases: {0}", header.aliasCount)
                    });
                }

                return results;
            }

            private static SoundSourceObj ConvertAliasesToSoundSourceObj(SndAliasList[] aliases, HydraInstance instance)
            {
                var sourceFile = new SoundSourceObj()
                {
                    Aliases = new SoundSourceObj.Alias[aliases.Length]
                };

                for (int i = 0; i < aliases.Length; i++)
                {
                    var aliasName = instance.Reader.ReadNullTerminatedString(aliases[i].name);
                    var aliasEntries = instance.Reader.ReadArray<SndAlias>(aliases[i].head, aliases[i].count);

                    sourceFile.Aliases[i] = new SoundSourceObj.Alias()
                    {
                        Entries = new SoundSourceObj.Alias.Entry[aliases[i].count]
                    };

                    for (int j = 0; j < aliases[i].count; j++)
                    {
                        var entry = aliasEntries[j];

                        sourceFile.Aliases[i].Entries[j] = new SoundSourceObj.Alias.Entry();

                        sourceFile.Aliases[i].Entries[j].Name = aliasName;
                        sourceFile.Aliases[i].Entries[j].Secondary = entry.secondaryname != 0 ? instance.Reader.ReadNullTerminatedString(entry.secondaryname) : "";
                        sourceFile.Aliases[i].Entries[j].Subtitle = entry.subtitle != 0 ? instance.Reader.ReadNullTerminatedString(entry.subtitle) : "";
                        sourceFile.Aliases[i].Entries[j].FileSpec = instance.Game.CleanAssetName(HydraAssetType.Sound, instance.Reader.ReadNullTerminatedString(entry.assetFileName));

                        sourceFile.Aliases[i].Entries[j].DryMinCurve = CURVES[(entry.flags1 >> 0xE) & 0x3F];
                        sourceFile.Aliases[i].Entries[j].DryMaxCurve = CURVES[(entry.flags1 >> 0x2) & 0x3F];
                        sourceFile.Aliases[i].Entries[j].WetMinCurve = CURVES[(entry.flags1 >> 0x14) & 0x3F];
                        sourceFile.Aliases[i].Entries[j].WetMinCurve = CURVES[(entry.flags1 >> 0x8) & 0x3F];

                        sourceFile.Aliases[i].Entries[j].Storage            = LOAD_TYPES[(entry.flags0 >> 15) & 0x3];
                        sourceFile.Aliases[i].Entries[j].VolumeGroup        = GROUPS[(entry.flags0 >> 17) & 0x1F];
                        sourceFile.Aliases[i].Entries[j].FluxType           = MOVE_TYPES[(entry.flags0 >> 22) & 0x7];
                        sourceFile.Aliases[i].Entries[j].LimitType          = LIMIT_TYPES[(entry.flags0 >> 25) & 0x3];
                        sourceFile.Aliases[i].Entries[j].EntityLimitType    = LIMIT_TYPES[(entry.flags0 >> 27) & 0x3];
                        //sourceFile.Aliases[i].Entries[j].RandomizeType      = RANDOMIZE_TYPES[(entry.flags0 >> 29) & 0x7];

                        sourceFile.Aliases[i].Entries[j].Looping        = LOOP_TYPES[Bytes.GetBit(entry.flags0, 0)];
                        sourceFile.Aliases[i].Entries[j].PanType        = PAN_TYPES[Bytes.GetBit(entry.flags0, 1)];
                        sourceFile.Aliases[i].Entries[j].DistanceLpf    = NO_YES[Bytes.GetBit(entry.flags0, 2)];
                        sourceFile.Aliases[i].Entries[j].Doppler        = NO_YES[Bytes.GetBit(entry.flags0, 3)];
                        sourceFile.Aliases[i].Entries[j].IsBig          = NO_YES[Bytes.GetBit(entry.flags0, 4)];
                        sourceFile.Aliases[i].Entries[j].Pauseable      = NO_YES[Bytes.GetBit(entry.flags0, 5)];
                        sourceFile.Aliases[i].Entries[j].IsMusic        = NO_YES[Bytes.GetBit(entry.flags0, 6)];
                        sourceFile.Aliases[i].Entries[j].StopOnEntDeath = NO_YES[Bytes.GetBit(entry.flags0, 7)];
                        sourceFile.Aliases[i].Entries[j].Timescale      = NO_YES[Bytes.GetBit(entry.flags0, 8)];
                        sourceFile.Aliases[i].Entries[j].VoiceLimit     = NO_YES[Bytes.GetBit(entry.flags0, 9)];
                        sourceFile.Aliases[i].Entries[j].IgnoreMaxDist  = NO_YES[Bytes.GetBit(entry.flags0, 10)];
                        sourceFile.Aliases[i].Entries[j].Bus            = BUS_IDS[(entry.flags0 >> 11) & 0xF];

                        sourceFile.Aliases[i].Entries[j].NeverPlayTwice = NO_YES[Bytes.GetBit(entry.flags1, 0)];
                        sourceFile.Aliases[i].Entries[j].IsCinematic    = NO_YES[Bytes.GetBit(entry.flags1, 27)];

                        sourceFile.Aliases[i].Entries[j].ReverbSend = Calculate100Value(entry.reverbSend);
                        sourceFile.Aliases[i].Entries[j].CenterSend = Calculate100Value(entry.centerSend);
                        sourceFile.Aliases[i].Entries[j].VolMin = Calculate100Value(entry.volMin);
                        sourceFile.Aliases[i].Entries[j].VolMax = Calculate100Value(entry.volMax);
                        sourceFile.Aliases[i].Entries[j].EnvelopPercent = Calculate100Value(entry.envelopPercentage);

                        sourceFile.Aliases[i].Entries[j].PitchMin = CalculatePitch(entry.pitchMin);
                        sourceFile.Aliases[i].Entries[j].PitchMax = CalculatePitch(entry.pitchMax);



                        sourceFile.Aliases[i].Entries[j].DistMin = (entry.distMin * 2);
                        sourceFile.Aliases[i].Entries[j].DistMaxDry = (entry.distMax * 2);
                        sourceFile.Aliases[i].Entries[j].DistMaxWet = (entry.distReverbMax * 2);
                        sourceFile.Aliases[i].Entries[j].EnvelopMin = (entry.envelopMin * 2);
                        sourceFile.Aliases[i].Entries[j].EnvelopMax = (entry.envelopMax * 2);

                        sourceFile.Aliases[i].Entries[j].StartDelay = entry.startDelay;
                        sourceFile.Aliases[i].Entries[j].FadeIn = entry.fadeIn;
                        sourceFile.Aliases[i].Entries[j].FadeOut = entry.fadeOut;

                        //sourceFile.Aliases[i].Entries[j].StopOnPlay = GetHashedString(entry.stopOnPlay);
                        //sourceFile.Aliases[i].Entries[j].FutzPatch = GetHashedString(entry.futzPatch);

                        //sourceFile.Aliases[i].Entries[j].Duck = GetHashedString(entry.duck);
                        //sourceFile.Aliases[i].Entries[j].ContextType = GetHashedString(entry.contextType);
                        //sourceFile.Aliases[i].Entries[j].ContextValue = GetHashedString(entry.contextValue);

                        sourceFile.Aliases[i].Entries[j].PriorityThresholdMin = Math.Round(entry.minPriorityThreshold / 255.0, 2);
                        sourceFile.Aliases[i].Entries[j].PriorityThresholdMax = Math.Round(entry.maxPriorityThreshold / 255.0, 2);
                        sourceFile.Aliases[i].Entries[j].PriorityMin = entry.minPriority;
                        sourceFile.Aliases[i].Entries[j].PriorityMax = entry.maxPriority;


                        sourceFile.Aliases[i].Entries[j].Probability = Math.Round(entry.probability / 255.0, 2);
                        sourceFile.Aliases[i].Entries[j].OcclusionLevel = Math.Round(entry.occlusionLevel / 255.0, 2);

                        sourceFile.Aliases[i].Entries[j].Pan = PANS[entry.pan];
                        sourceFile.Aliases[i].Entries[j].DuckGroup = DUCK_GROUPS[entry.duckGroup];

                        sourceFile.Aliases[i].Entries[j].FluxTime = entry.fluxTime;

                        sourceFile.Aliases[i].Entries[j].LimitCount = entry.limitCount;
                        sourceFile.Aliases[i].Entries[j].EntityLimitCount = entry.entityLimitCount;
                    }
                }

                return sourceFile;
            }

            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<SndBank>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.name))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                Directory.CreateDirectory(instance.SoundZoneFolder);

                var sourceObj = ConvertAliasesToSoundSourceObj(instance.Reader.ReadArray<SndAliasList>(header.alias, (int)header.aliasCount), instance);

                sourceObj.Save(Path.Combine(instance.SoundZoneFolder, asset.Name + ".alias.csv"));
            }

            public bool IsNullAsset(long nameAddress)
            {
                return nameAddress >= StartAddress && nameAddress <= EndAddress || nameAddress == 0;
            }
        }
    }
}
