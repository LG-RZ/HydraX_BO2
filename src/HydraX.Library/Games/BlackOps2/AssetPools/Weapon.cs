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
    using Pointer = UInt32;

    public partial class BlackOps2
    {
        private class Weapon : IAssetPool
        {
            #region Enums

            public enum weaponIconRatioType_t 
            {
                WEAPON_ICON_RATIO_1TO1=0,
                WEAPON_ICON_RATIO_2TO1=1,
                WEAPON_ICON_RATIO_4TO1=2,
                WEAPON_ICON_RATIO_COUNT=3
            }

            private readonly static string[] weapIconRatioNames =
            {
                "1:1",
                "2:1",
                "4:1",
            };
            
            public enum weapType_t 
            {
                WEAPTYPE_BINOCULARS=3,
                WEAPTYPE_BOMB=5,
                WEAPTYPE_BULLET=0,
                WEAPTYPE_GAS=4,
                WEAPTYPE_GRENADE=1,
                WEAPTYPE_MELEE=7,
                WEAPTYPE_MINE=6,
                WEAPTYPE_NUM=9,
                WEAPTYPE_PROJECTILE=2,
                WEAPTYPE_RIOTSHIELD=8
            }
            
            public enum weapClass_t 
            {
                WEAPCLASS_GAS=9,
                WEAPCLASS_GRENADE=5,
                WEAPCLASS_ITEM=10,
                WEAPCLASS_KILLSTREAK_ALT_STORED_WEAPON=12,
                WEAPCLASS_MELEE=11,
                WEAPCLASS_MG=1,
                WEAPCLASS_NON_PLAYER=8,
                WEAPCLASS_NUM=14,
                WEAPCLASS_PISTOL=4,
                WEAPCLASS_PISTOL_SPREAD=13,
                WEAPCLASS_RIFLE=0,
                WEAPCLASS_ROCKETLAUNCHER=6,
                WEAPCLASS_SMG=2,
                WEAPCLASS_SPREAD=3,
                WEAPCLASS_TURRET=7
            }
            
            public enum PenetrateType 
            {
                PENETRATE_TYPE_COUNT=4,
                PENETRATE_TYPE_LARGE=3,
                PENETRATE_TYPE_MEDIUM=2,
                PENETRATE_TYPE_NONE=0,
                PENETRATE_TYPE_SMALL=1
            }
            
            public enum ImpactType 
            {
                IMPACT_TYPE_BLADE=15,
                IMPACT_TYPE_BOLT=14,
                IMPACT_TYPE_BULLET_AP=3,
                IMPACT_TYPE_BULLET_LARGE=2,
                IMPACT_TYPE_BULLET_SMALL=1,
                IMPACT_TYPE_BULLET_XTREME=4,
                IMPACT_TYPE_COUNT=16,
                IMPACT_TYPE_GRENADE_BOUNCE=6,
                IMPACT_TYPE_GRENADE_EXPLODE=7,
                IMPACT_TYPE_MORTAR_SHELL=12,
                IMPACT_TYPE_NONE=0,
                IMPACT_TYPE_PROJECTILE_DUD=11,
                IMPACT_TYPE_RIFLE_GRENADE=8,
                IMPACT_TYPE_ROCKET_EXPLODE=9,
                IMPACT_TYPE_ROCKET_EXPLODE_XTREME=10,
                IMPACT_TYPE_SHOTGUN=5,
                IMPACT_TYPE_TANK_SHELL=13
            }
            
            public enum weapInventoryType_t 
            {
                WEAPINVENTORYCOUNT=6,
                WEAPINVENTORY_ALTMODE=3,
                WEAPINVENTORY_DWLEFTHAND=5,
                WEAPINVENTORY_ITEM=2,
                WEAPINVENTORY_MELEE=4,
                WEAPINVENTORY_OFFHAND=1,
                WEAPINVENTORY_PRIMARY=0
            }
            
            public enum weapFireType_t 
            {
                WEAPON_FIRETYPECOUNT=10,
                WEAPON_FIRETYPE_BURSTFIRE2=2,
                WEAPON_FIRETYPE_BURSTFIRE3=3,
                WEAPON_FIRETYPE_BURSTFIRE4=4,
                WEAPON_FIRETYPE_BURSTFIRE5=5,
                WEAPON_FIRETYPE_CHARGESHOT=8,
                WEAPON_FIRETYPE_FULLAUTO=0,
                WEAPON_FIRETYPE_JETGUN=9,
                WEAPON_FIRETYPE_MINIGUN=7,
                WEAPON_FIRETYPE_SINGLESHOT=1,
                WEAPON_FIRETYPE_STACKED=6
            }
            
            public enum weapClipType_t 
            {
                WEAPON_CLIPTYPECOUNT=6,
                WEAPON_CLIPTYPE_BOTTOM=0,
                WEAPON_CLIPTYPE_DP28=3,
                WEAPON_CLIPTYPE_LEFT=2,
                WEAPON_CLIPTYPE_LMG=5,
                WEAPON_CLIPTYPE_PTRS=4,
                WEAPON_CLIPTYPE_TOP=1
            }
            
            public enum barrelType_t 
            {
                BARREL_TYPE_COUNT=6,
                BARREL_TYPE_DUAL=1,
                BARREL_TYPE_DUAL_ALTERNATE=2,
                BARREL_TYPE_QUAD=3,
                BARREL_TYPE_QUAD_ALTERNATE=4,
                BARREL_TYPE_QUAD_DOUBLE_ALTERNATE=5,
                BARREL_TYPE_SINGLE=0
            }
            
            public enum OffhandClass 
            {
                OFFHAND_CLASS_COUNT=6,
                OFFHAND_CLASS_FLASH_GRENADE=3,
                OFFHAND_CLASS_FRAG_GRENADE=1,
                OFFHAND_CLASS_GEAR=4,
                OFFHAND_CLASS_NONE=0,
                OFFHAND_CLASS_SMOKE_GRENADE=2,
                OFFHAND_CLASS_SUPPLYDROP_MARKER=5
            }
            
            public enum OffhandSlot 
            {
                OFFHAND_SLOT_COUNT=5,
                OFFHAND_SLOT_EQUIPMENT=3,
                OFFHAND_SLOT_LETHAL_GRENADE=1,
                OFFHAND_SLOT_NONE=0,
                OFFHAND_SLOT_SPECIFIC_USE=4,
                OFFHAND_SLOT_TACTICAL_GRENADE=2
            }
            
            public enum weapStance_t 
            {
                WEAPSTANCE_DUCK=1,
                WEAPSTANCE_NUM=3,
                WEAPSTANCE_PRONE=2,
                WEAPSTANCE_STAND=0
            }

            public enum weapOverlayReticle_t
            {
                WEAPOVERLAYRETICLE_CROSSHAIR = 1,
                WEAPOVERLAYRETICLE_NONE = 0,
                WEAPOVERLAYRETICLE_NUM = 2
            }

            public enum WeapOverlayInteface_t
            {
                WEAPOVERLAYINTERFACECOUNT = 3,
                WEAPOVERLAYINTERFACE_JAVELIN = 1,
                WEAPOVERLAYINTERFACE_NONE = 0,
                WEAPOVERLAYINTERFACE_TURRETSCOPE = 2
            }

            public enum WeapStickinessType
            {
                WEAPSTICKINESS_ALL = 1,
                WEAPSTICKINESS_ALL_NO_SENTIENTS = 2,
                WEAPSTICKINESS_COUNT = 6,
                WEAPSTICKINESS_FLESH = 5,
                WEAPSTICKINESS_GROUND = 3,
                WEAPSTICKINESS_GROUND_WITH_YAW = 4,
                WEAPSTICKINESS_NONE = 0
            }

            public enum WeapRotateType
            {
                WEAPROTATE_BLADE_ROTATE = 1,
                WEAPROTATE_COUNT = 3,
                WEAPROTATE_CYLINDER_ROTATE = 2,
                WEAPROTATE_GRENADE_ROTATE = 0
            }

            public enum activeReticleType_t
            {
                VEH_ACTIVE_RETICLE_BOUNCING_DIAMOND = 2,
                VEH_ACTIVE_RETICLE_COUNT = 4,
                VEH_ACTIVE_RETICLE_MISSILE_LOCK = 3,
                VEH_ACTIVE_RETICLE_NONE = 0,
                VEH_ACTIVE_RETICLE_PIP_ON_A_STICK = 1
            }

            public enum guidedMissileType_t
            {
                MISSILE_GUIDANCE_BALLISTIC = 4,
                MISSILE_GUIDANCE_COUNT = 9,
                MISSILE_GUIDANCE_DRONE = 7,
                MISSILE_GUIDANCE_HEATSEEKING = 8,
                MISSILE_GUIDANCE_HELLFIRE = 2,
                MISSILE_GUIDANCE_JAVELIN = 3,
                MISSILE_GUIDANCE_NONE = 0,
                MISSILE_GUIDANCE_SIDEWINDER = 1,
                MISSILE_GUIDANCE_TVGUIDED = 6,
                MISSILE_GUIDANCE_WIREGUIDED = 5
            }

            public enum weapProjExposion_t
            {
                WEAPPROJEXP_BOLT = 9,
                WEAPPROJEXP_DUD = 4,
                WEAPPROJEXP_FIRE = 7,
                WEAPPROJEXP_FLASHBANG = 2,
                WEAPPROJEXP_GRENADE = 0,
                WEAPPROJEXP_HEAVY = 6,
                WEAPPROJEXP_NAPALMBLOB = 8,
                WEAPPROJEXP_NONE = 3,
                WEAPPROJEXP_NUM = 11,
                WEAPPROJEXP_ROCKET = 1,
                WEAPPROJEXP_SHRAPNELSPAN = 10,
                WEAPPROJEXP_SMOKE = 5
            }

            public enum ammoCounterClipType_t
            {
                AMMO_COUNTER_CLIP_ALTWEAPON = 6,
                AMMO_COUNTER_CLIP_BELTFED = 5,
                AMMO_COUNTER_CLIP_COUNT = 7,
                AMMO_COUNTER_CLIP_MAGAZINE = 1,
                AMMO_COUNTER_CLIP_NONE = 0,
                AMMO_COUNTER_CLIP_ROCKET = 4,
                AMMO_COUNTER_CLIP_SHORTMAGAZINE = 2,
                AMMO_COUNTER_CLIP_SHOTGUN = 3
            }

            #endregion

            #region AssetStructures
            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x8)]
            private struct vec2_t
            {
                public float X;
                public float Y;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xC)]
            private struct vec3_t
            {
                public float X;
                public float Y;
                public float Z;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0x2CC)]
            private unsafe struct WeaponVariantDef
            {
                public Pointer szInternalName; // char *
                public int iVariantCount;
                public Pointer weapDef; // WeaponDef * 
                public Pointer szDisplayName; // char *
                public Pointer szAltWeaponName; // char *
                public Pointer szAttachmentUnique; // char *
                public Pointer attachments; // WeaponAttachment **
                public Pointer attachmentUniques; // WeaponAttachmentUnique **
                public Pointer szXAnims; // char **
                public Pointer hideTags; // ushort *
                public Pointer attachViewModel; // XModel **
                public Pointer attachWorldModel; // XModel **
                public Pointer attachViewModelTag; // char **
                public Pointer attachWorldModelTag; // char **
                public fixed float attachViewModelOffsets[24];
                public fixed float attachWorldModelOffsets[24];
                public fixed float attachViewModelRotations[24];
                public fixed float attachWorldModelRotations[24];
                public vec3_t stowedModelOffsets;
                public vec3_t stowedModelRotations;
                public uint altWeaponIndex;
                public int iAttachments;
                [MarshalAs(UnmanagedType.U1)] public byte bIgnoreAttachments;
                public int iClipSize;
                public int iReloadTime;
                public int iReloadEmptyTime;
                public int iReloadQuickTime;
                public int iReloadQuickEmptyTime;
                public int iAdsTransInTime;
                public int iAdsTransOutTime;
                public int iAltRaiseTime;
                public Pointer szAmmoDisplayName; // char *
                public Pointer szAmmoName; // char *
                public int iAmmoIndex;
                public Pointer szClipName; // char *
                public int iClipIndex;
                public float fAimAssistRangeAds;
                public float fAdsSwayHorizScale;
                public float fAdsSwayVertScale;
                public float fAdsViewKickCenterSpeed;
                public float fHipViewKickCenterSpeed;
                public float fAdsZoomFov1;
                public float fAdsZoomFov2;
                public float fAdsZoomFov3;
                public float fAdsZoomInFrac;
                public float fAdsZoomOutFrac;
                public float fOverlayAlphaScale;
                public fixed float fOOPosAnimLength[2];
                [MarshalAs(UnmanagedType.U1)] public byte bSilenced;
                [MarshalAs(UnmanagedType.U1)] public byte bDualMag;
                [MarshalAs(UnmanagedType.U1)] public byte bInfraRed;
                [MarshalAs(UnmanagedType.U1)] public byte bTVGuided;
                public fixed uint perks[2];
                [MarshalAs(UnmanagedType.U1)] public byte bAntiQuickScope;
                public Pointer overlayMaterial; // Material *
                public Pointer overlayMaterialLowRes; // Material *
                public Pointer dpadIcon; // Material *
                public weaponIconRatioType_t dpadIconRatio;
                [MarshalAs(UnmanagedType.U1)] public byte noAmmoOnDpadIcon;
                [MarshalAs(UnmanagedType.U1)] public byte mmsWeapon;
                [MarshalAs(UnmanagedType.U1)] public byte mmsInScope;
                public float mmsFOV;
                public float mmsAspect;
                public float mmsMaxDist;
                public vec3_t ikLeftHandIdlePos;
                public vec3_t ikLeftHandOffset;
                public vec3_t ikLeftHandRotation;
                [MarshalAs(UnmanagedType.U1)] public byte bUsingLeftHandProneIK;
                public vec3_t ikLeftHandProneOffset;
                public vec3_t ikLeftHandProneRotation;
                public vec3_t ikLeftHandUiViewerOffset;
                public vec3_t ikLeftHandUiViewerRotation;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0x990)]
            private unsafe struct WeaponDef 
            {
                public Pointer szOverlayName; // char *
                public Pointer gunXModel; // XModel **
                public Pointer handXModel; // XModel *
                public Pointer szModeName; // char *
                public Pointer notetrackSoundMapKeys; // ushort *
                public Pointer notetrackSoundMapValues; // ushort *
                public int playerAnimType;
                public weapType_t weapType;
                public weapClass_t weapClass;
                public PenetrateType penetrateType;
                public ImpactType impactType;
                public weapInventoryType_t inventoryType;
                public weapFireType_t fireType;
                public weapClipType_t clipType;
                public barrelType_t barrelType;
                public int itemIndex;
                public Pointer parentWeaponName; // char *
                public int iJamFireTime;
                public int overheatWeapon;
                public float overheatRate;
                public float cooldownRate;
                public float overheatEndVal;
                [MarshalAs(UnmanagedType.U1)] public byte coolWhileFiring;
                [MarshalAs(UnmanagedType.U1)] public byte fuelTankWeapon;
                public int iTankLifeTime;
                public OffhandClass offhandClass;
                public OffhandSlot offhandSlot;
                public weapStance_t stance;
                public Pointer viewFlashEffect; // FxEffectDef *
                public Pointer worldFlashEffect; // FxEffectDef *
                public Pointer barrelCooldownEffect; // FxEffectDef *
                public int barrelCooldownMinCount;
                public vec3_t vViewFlashOffset;
                public vec3_t vWorldFlashOffset;
                public Pointer pickupSound; // char *
                public Pointer pickupSoundPlayer; // char *
                public Pointer ammoPickupSound; // char *
                public Pointer ammoPickupSoundPlayer; // char *
                public Pointer projectileSound; // char *
                public Pointer pullbackSound; // char *
                public Pointer pullbackSoundPlayer; // char *
                public Pointer fireSound; // char *
                public Pointer fireSoundPlayer; // char *
                public Pointer fireLoopSound; // char *
                public Pointer fireLoopSoundPlayer; // char *
                public Pointer fireLoopEndSound; // char *
                public Pointer fireLoopEndSoundPlayer; // char *
                public Pointer fireStartSound; // char *
                public Pointer fireStopSound; // char *
                public Pointer fireKillcamSound; // char *
                public Pointer fireStartSoundPlayer; // char *
                public Pointer fireStopSoundPlayer; // char *
                public Pointer fireKillcamSoundPlayer; // char *
                public Pointer fireLastSound; // char *
                public Pointer fireLastSoundPlayer; // char *
                public Pointer emptyFireSound; // char *
                public Pointer emptyFireSoundPlayer; // char *
                public Pointer crackSound; // char *
                public Pointer whizbySound; // char *
                public Pointer meleeSwipeSound; // char *
                public Pointer meleeSwipeSoundPlayer; // char *
                public Pointer meleeHitSound; // char *
                public Pointer meleeMissSound; // char *
                public Pointer rechamberSound; // char *
                public Pointer rechamberSoundPlayer; // char *
                public Pointer reloadSound; // char *
                public Pointer reloadSoundPlayer; // char *
                public Pointer reloadEmptySound; // char *
                public Pointer reloadEmptySoundPlayer; // char *
                public Pointer reloadStartSound; // char *
                public Pointer reloadStartSoundPlayer; // char *
                public Pointer reloadEndSound; // char *
                public Pointer reloadEndSoundPlayer; // char *
                public Pointer rotateLoopSound; // char *
                public Pointer rotateLoopSoundPlayer; // char *
                public Pointer rotateStopSound; // char *
                public Pointer rotateStopSoundPlayer; // char *
                public Pointer deploySound; // char *
                public Pointer deploySoundPlayer; // char *
                public Pointer finishDeploySound; // char *
                public Pointer finishDeploySoundPlayer; // char *
                public Pointer breakdownSound; // char *
                public Pointer breakdownSoundPlayer; // char *
                public Pointer finishBreakdownSound; // char *
                public Pointer finishBreakdownSoundPlayer; // char *
                public Pointer detonateSound; // char *
                public Pointer detonateSoundPlayer; // char *
                public Pointer nightVisionWearSound; // char *
                public Pointer nightVisionWearSoundPlayer; // char *
                public Pointer nightVisionRemoveSound; // char *
                public Pointer nightVisionRemoveSoundPlayer; // char *
                public Pointer altSwitchSound; // char *
                public Pointer altSwitchSoundPlayer; // char *
                public Pointer raiseSound; // char *
                public Pointer raiseSoundPlayer; // char *
                public Pointer firstRaiseSound; // char *
                public Pointer firstRaiseSoundPlayer; // char *
                public Pointer adsRaiseSoundPlayer; // char *
                public Pointer adsLowerSoundPlayer; // char *
                public Pointer putawaySound; // char *
                public Pointer putawaySoundPlayer; // char *
                public Pointer overheatSound; // char *
                public Pointer overheatSoundPlayer; // char *
                public Pointer adsZoomSound; // char *
                public Pointer shellCasing; // char *
                public Pointer shellCasingPlayer; // char *
                public Pointer bounceSound; // char **
                public Pointer standMountedWeapdef; // char *
                public Pointer crouchMountedWeapdef; // char *
                public Pointer proneMountedWeapdef; // char *
                public int standMountedIndex;
                public int crouchMountedIndex;
                public int proneMountedIndex;
                public Pointer viewShellEjectEffect; // FxEffectDef *
                public Pointer worldShellEjectEffect; // FxEffectDef *
                public Pointer viewLastShotEjectEffect; // FxEffectDef *
                public Pointer worldLastShotEjectEffect; // FxEffectDef *
                public vec3_t vViewShellEjectOffset;
                public vec3_t vWorldShellEjectOffset;
                public vec3_t vViewShellEjectRotation;
                public vec3_t vWorldShellEjectRotation;
                public Pointer reticleCenter; // Material *
                public Pointer reticleSide; // Material *
                public int iReticleCenterSize;
                public int iReticleSideSize;
                public int iReticleMinOfs;
                public activeReticleType_t activeReticleType;
                public vec3_t vStandMove;
                public vec3_t vStandRot;
                public vec3_t vDuckedOfs;
                public vec3_t vDuckedMove;
                public vec3_t vDuckedSprintOfs;
                public vec3_t vDuckedSprintRot;
                public vec2_t vDuckedSprintBob;
                public float fDuckedSprintCycleScale;
                public vec3_t vSprintOfs;
                public vec3_t vSprintRot;
                public vec2_t vSprintBob;
                public float fSprintCycleScale;
                public vec3_t vLowReadyOfs;
                public vec3_t vLowReadyRot;
                public vec3_t vRideOfs;
                public vec3_t vRideRot;
                public vec3_t vDtpOfs;
                public vec3_t vDtpRot;
                public vec2_t vDtpBob;
                public float fDtpCycleScale;
                public vec3_t vMantleOfs;
                public vec3_t vMantleRot;
                public vec3_t vSlideOfs;
                public vec3_t vSlideRot;
                public vec3_t vDuckedRot;
                public vec3_t vProneOfs;
                public vec3_t vProneMove;
                public vec3_t vProneRot;
                public vec3_t vStrafeMove;
                public vec3_t vStrafeRot;
                public float fPosMoveRate;
                public float fPosProneMoveRate;
                public float fStandMoveMinSpeed;
                public float fDuckedMoveMinSpeed;
                public float fProneMoveMinSpeed;
                public float fPosRotRate;
                public float fPosProneRotRate;
                public float fStandRotMinSpeed;
                public float fDuckedRotMinSpeed;
                public float fProneRotMinSpeed;
                public Pointer worldModel; // XModel **
                public Pointer worldClipModel; // XModel *
                public Pointer rocketModel; // XModel *
                public Pointer mountedModel; // XModel *
                public Pointer additionalMeleeModel; // XModel *
                public Pointer fireTypeIcon; // Material *
                public Pointer hudIcon; // Material *
                public weaponIconRatioType_t hudIconRatio;
                public Pointer indicatorIcon; // Material *
                public weaponIconRatioType_t indicatorIconRatio;
                public Pointer ammoCounterIcon; // Material *
                public weaponIconRatioType_t ammoCounterIconRatio;
                public ammoCounterClipType_t ammoCounterClip;
                public int iStartAmmo;
                public int iMaxAmmo;
                public int shotCount;
                public Pointer szSharedAmmoCapName; // char *
                public int iSharedAmmoCapIndex;
                public int iSharedAmmoCap;
                [MarshalAs(UnmanagedType.U1)] public byte unlimitedAmmo;
                [MarshalAs(UnmanagedType.U1)] public byte ammoCountClipRelative;
                public fixed int damage[6];
                public fixed float damageRange[6];
                public int minPlayerDamage;
                public float damageDuration;
                public float damageInterval;
                public int playerDamage;
                public int iMeleeDamage;
                public int iDamageType;
                public ushort explosionTag;
                public int iFireDelay;
                public int iMeleeDelay;
                public int meleeChargeDelay;
                public int iDetonateDelay;
                public int iSpinUpTime;
                public int iSpinDownTime;
                public float spinRate;
                public Pointer spinLoopSound; // char *
                public Pointer spinLoopSoundPlayer; // char *
                public Pointer startSpinSound; // char *
                public Pointer startSpinSoundPlayer; // char *
                public Pointer stopSpinSound; // char *
                public Pointer stopSpinSoundPlayer; // char *
                [MarshalAs(UnmanagedType.U1)] public byte applySpinPitch;
                public int iFireTime;
                public int iLastFireTime;
                public int iRechamberTime;
                public int iRechamberBoltTime;
                public int iHoldFireTime;
                public int iDetonateTime;
                public int iMeleeTime;
                public int iBurstDelayTime;
                public int meleeChargeTime;
                public int iReloadTimeRight;
                public int iReloadTimeLeft;
                public int reloadShowRocketTime;
                public int iReloadEmptyTimeLeft;
                public int iReloadAddTime;
                public int iReloadEmptyAddTime;
                public int iReloadQuickAddTime;
                public int iReloadQuickEmptyAddTime;
                public int iReloadStartTime;
                public int iReloadStartAddTime;
                public int iReloadEndTime;
                public int iDropTime;
                public int iRaiseTime;
                public int iAltDropTime;
                public int quickDropTime;
                public int quickRaiseTime;
                public int iFirstRaiseTime;
                public int iEmptyRaiseTime;
                public int iEmptyDropTime;
                public int sprintInTime;
                public int sprintLoopTime;
                public int sprintOutTime;
                public int lowReadyInTime;
                public int lowReadyLoopTime;
                public int lowReadyOutTime;
                public int contFireInTime;
                public int contFireLoopTime;
                public int contFireOutTime;
                public int dtpInTime;
                public int dtpLoopTime;
                public int dtpOutTime;
                public int crawlInTime;
                public int crawlForwardTime;
                public int crawlBackTime;
                public int crawlRightTime;
                public int crawlLeftTime;
                public int crawlOutFireTime;
                public int crawlOutTime;
                public int slideInTime;
                public int deployTime;
                public int breakdownTime;
                public int iFlourishTime;
                public int nightVisionWearTime;
                public int nightVisionWearTimeFadeOutEnd;
                public int nightVisionWearTimePowerUp;
                public int nightVisionRemoveTime;
                public int nightVisionRemoveTimePowerDown;
                public int nightVisionRemoveTimeFadeInStart;
                public int fuseTime;
                public int aiFuseTime;
                public int lockOnRadius;
                public int lockOnSpeed;
                [MarshalAs(UnmanagedType.U1)] public byte requireLockonToFire;
                [MarshalAs(UnmanagedType.U1)] public byte noAdsWhenMagEmpty;
                [MarshalAs(UnmanagedType.U1)] public byte avoidDropCleanup;
                public uint stackFire;
                public float stackFireSpread;
                public float stackFireAccuracyDecay;
                public Pointer stackSound; // char *
                public float autoAimRange;
                public float aimAssistRange;
                [MarshalAs(UnmanagedType.U1)] public byte mountableWeapon;
                public float aimPadding;
                public float enemyCrosshairRange;
                [MarshalAs(UnmanagedType.U1)] public byte crosshairColorChange;
                public float moveSpeedScale;
                public float adsMoveSpeedScale;
                public float sprintDurationScale;
                public weapOverlayReticle_t overlayReticle;
                public WeapOverlayInteface_t overlayInterface;
                public float overlayWidth;
                public float overlayHeight;
                public float fAdsBobFactor;
                public float fAdsViewBobMult;
                [MarshalAs(UnmanagedType.U1)] public byte bHoldBreathToSteady;
                public float fHipSpreadStandMin;
                public float fHipSpreadDuckedMin;
                public float fHipSpreadProneMin;
                public float hipSpreadStandMax;
                public float hipSpreadDuckedMax;
                public float hipSpreadProneMax;
                public float fHipSpreadDecayRate;
                public float fHipSpreadFireAdd;
                public float fHipSpreadTurnAdd;
                public float fHipSpreadMoveAdd;
                public float fHipSpreadDuckedDecay;
                public float fHipSpreadProneDecay;
                public float fHipReticleSidePos;
                public float fAdsIdleAmount;
                public float fHipIdleAmount;
                public float adsIdleSpeed;
                public float hipIdleSpeed;
                public float fIdleCrouchFactor;
                public float fIdleProneFactor;
                public float fGunMaxPitch;
                public float fGunMaxYaw;
                public float swayMaxAngle;
                public float swayLerpSpeed;
                public float swayPitchScale;
                public float swayYawScale;
                public float swayHorizScale;
                public float swayVertScale;
                public float swayShellShockScale;
                public float adsSwayMaxAngle;
                public float adsSwayLerpSpeed;
                public float adsSwayPitchScale;
                public float adsSwayYawScale;
                [MarshalAs(UnmanagedType.U1)] public byte sharedAmmo;
                [MarshalAs(UnmanagedType.U1)] public byte bRifleBullet;
                [MarshalAs(UnmanagedType.U1)] public byte armorPiercing;
                [MarshalAs(UnmanagedType.U1)] public byte bAirburstWeapon;
                [MarshalAs(UnmanagedType.U1)] public byte bBoltAction;
                [MarshalAs(UnmanagedType.U1)] public byte bUseAltTagFlash;
                [MarshalAs(UnmanagedType.U1)] public byte bUseAntiLagRewind;
                [MarshalAs(UnmanagedType.U1)] public byte bIsCarriedKillstreakWeapon;
                [MarshalAs(UnmanagedType.U1)] public byte aimDownSight;
                [MarshalAs(UnmanagedType.U1)] public byte bRechamberWhileAds;
                [MarshalAs(UnmanagedType.U1)] public byte bReloadWhileAds;
                public float adsViewErrorMin;
                public float adsViewErrorMax;
                [MarshalAs(UnmanagedType.U1)] public byte bCookOffHold;
                [MarshalAs(UnmanagedType.U1)] public byte bClipOnly;
                [MarshalAs(UnmanagedType.U1)] public byte bCanUseInVehicle;
                [MarshalAs(UnmanagedType.U1)] public byte bNoDropsOrRaises;
                [MarshalAs(UnmanagedType.U1)] public byte adsFireOnly;
                [MarshalAs(UnmanagedType.U1)] public byte cancelAutoHolsterWhenEmpty;
                [MarshalAs(UnmanagedType.U1)] public byte suppressAmmoReserveDisplay;
                [MarshalAs(UnmanagedType.U1)] public byte laserSight;
                [MarshalAs(UnmanagedType.U1)] public byte laserSightDuringNightvision;
                [MarshalAs(UnmanagedType.U1)] public byte bHideThirdPerson;
                [MarshalAs(UnmanagedType.U1)] public byte bHasBayonet;
                [MarshalAs(UnmanagedType.U1)] public byte bDualWield;
                [MarshalAs(UnmanagedType.U1)] public byte bExplodeOnGround;
                [MarshalAs(UnmanagedType.U1)] public byte bThrowBack;
                [MarshalAs(UnmanagedType.U1)] public byte bRetrievable;
                [MarshalAs(UnmanagedType.U1)] public byte bDieOnRespawn;
                [MarshalAs(UnmanagedType.U1)] public byte bNoThirdPersonDropsOrRaises;
                [MarshalAs(UnmanagedType.U1)] public byte bContinuousFire;
                [MarshalAs(UnmanagedType.U1)] public byte bNoPing;
                [MarshalAs(UnmanagedType.U1)] public byte bForceBounce;
                [MarshalAs(UnmanagedType.U1)] public byte bUseDroppedModelAsStowed;
                [MarshalAs(UnmanagedType.U1)] public byte bNoQuickDropWhenEmpty;
                [MarshalAs(UnmanagedType.U1)] public byte bKeepCrosshairWhenADS;
                [MarshalAs(UnmanagedType.U1)] public byte bUseOnlyAltWeaoponHideTagsInAltMode;
                [MarshalAs(UnmanagedType.U1)] public byte bAltWeaponAdsOnly;
                [MarshalAs(UnmanagedType.U1)] public byte bAltWeaponDisableSwitching;
                public Pointer killIcon; // Material *
                public weaponIconRatioType_t killIconRatio;
                [MarshalAs(UnmanagedType.U1)] public byte flipKillIcon;
                [MarshalAs(UnmanagedType.U1)] public byte bNoPartialReload;
                [MarshalAs(UnmanagedType.U1)] public byte bSegmentedReload;
                [MarshalAs(UnmanagedType.U1)] public byte bNoADSAutoReload;
                public int iReloadAmmoAdd;
                public int iReloadStartAdd;
                public Pointer szSpawnedGrenadeWeaponName; // char *
                public Pointer szDualWieldWeaponName; // char *
                public uint dualWieldWeaponIndex;
                public int iDropAmmoMin;
                public int iDropAmmoMax;
                public int iDropClipAmmoMin;
                public int iDropClipAmmoMax;
                public int iShotsBeforeRechamber;
                [MarshalAs(UnmanagedType.U1)] public byte blocksProne;
                [MarshalAs(UnmanagedType.U1)] public byte bShowIndicator;
                public int isRollingGrenade;
                public int useBallisticPrediction;
                public int isValuable;
                public int isTacticalInsertion;
                [MarshalAs(UnmanagedType.U1)] public byte isReviveWeapon;
                [MarshalAs(UnmanagedType.U1)] public byte bUseRigidBodyOnVehicle;
                public int iExplosionRadius;
                public int iExplosionRadiusMin;
                public int iIndicatorRadius;
                public int iExplosionInnerDamage;
                public int iExplosionOuterDamage;
                public float damageConeAngle;
                public int iProjectileSpeed;
                public int iProjectileSpeedUp;
                public int iProjectileSpeedRelativeUp;
                public int iProjectileSpeedForward;
                public float fProjectileTakeParentVelocity;
                public int iProjectileActivateDist;
                public float projLifetime;
                public float timeToAccelerate;
                public float projectileCurvature;
                public Pointer projectileModel; // XModel *
                public weapProjExposion_t projExplosion;
                public Pointer projExplosionEffect; // FxEffectDef *
                [MarshalAs(UnmanagedType.U1)]
                public byte projExplosionEffectForceNormalUp;
                public Pointer projExplosionEffect2; // FxEffectDef *
                [MarshalAs(UnmanagedType.U1)]
                public byte projExplosionEffect2ForceNormalUp;
                public Pointer projExplosionEffect3;// FxEffectDef *
                [MarshalAs(UnmanagedType.U1)]
                public byte projExplosionEffect3ForceNormalUp;
                public Pointer projExplosionEffect4;// FxEffectDef *
                [MarshalAs(UnmanagedType.U1)]
                public byte projExplosionEffect4ForceNormalUp;
                public Pointer projExplosionEffect5;// FxEffectDef *
                [MarshalAs(UnmanagedType.U1)]
                public byte projExplosionEffect5ForceNormalUp;
                public Pointer projDudEffect; // FxEffectDef *
                public Pointer projExplosionSound; // char *
                public Pointer projDudSound; // char *
                public Pointer mortarShellSound; // char *
                public Pointer tankShellSound; // char *
                [MarshalAs(UnmanagedType.U1)] public byte bProjImpactExplode;
                [MarshalAs(UnmanagedType.U1)] public byte bProjSentientImpactExplode;
                [MarshalAs(UnmanagedType.U1)] public byte bProjExplodeWhenStationary;
                [MarshalAs(UnmanagedType.U1)] public byte bBulletImpactExplode;
                public WeapStickinessType stickiness;
                public WeapRotateType rotateType;
                [MarshalAs(UnmanagedType.U1)] public byte plantable;
                [MarshalAs(UnmanagedType.U1)] public byte hasDetonator;
                [MarshalAs(UnmanagedType.U1)] public byte timedDetonation;
                [MarshalAs(UnmanagedType.U1)] public byte bNoCrumpleMissile;
                [MarshalAs(UnmanagedType.U1)] public byte rotate;
                [MarshalAs(UnmanagedType.U1)] public byte bKeepRolling;
                [MarshalAs(UnmanagedType.U1)] public byte holdButtonToThrow;
                [MarshalAs(UnmanagedType.U1)] public byte offhandHoldIsCancelable;
                [MarshalAs(UnmanagedType.U1)] public byte freezeMovementWhenFiring;
                public float lowAmmoWarningThreshold;
                [MarshalAs(UnmanagedType.U1)] public byte bDisallowAtMatchStart;
                public float meleeChargeRange;
                [MarshalAs(UnmanagedType.U1)] public byte bUseAsMelee;
                [MarshalAs(UnmanagedType.U1)] public byte isCameraSensor;
                [MarshalAs(UnmanagedType.U1)] public byte isAcousticSensor;
                [MarshalAs(UnmanagedType.U1)] public byte isLaserSensor;
                [MarshalAs(UnmanagedType.U1)] public byte isHoldUseGrenade;
                public Pointer parallelBounce; // float *
                public Pointer perpendicularBounce; // float *
                public Pointer projTrailEffect; // FxEffectDef *
                public vec3_t vProjectileColor;
                public guidedMissileType_t guidedMissileType;
                public float maxSteeringAccel;
                public int projIgnitionDelay;
                public Pointer projIgnitionEffect; // FxEffectDef *
                public Pointer projIgnitionSound; // cahr *
                public float fAdsAimPitch;
                public float fAdsCrosshairInFrac;
                public float fAdsCrosshairOutFrac;
                public int adsGunKickReducedKickBullets;
                public float adsGunKickReducedKickPercent;
                public float fAdsGunKickPitchMin;
                public float fAdsGunKickPitchMax;
                public float fAdsGunKickYawMin;
                public float fAdsGunKickYawMax;
                public float fAdsGunKickAccel;
                public float fAdsGunKickSpeedMax;
                public float fAdsGunKickSpeedDecay;
                public float fAdsGunKickStaticDecay;
                public float fAdsViewKickPitchMin;
                public float fAdsViewKickPitchMax;
                public float fAdsViewKickMinMagnitude;
                public float fAdsViewKickYawMin;
                public float fAdsViewKickYawMax;
                public float fAdsRecoilReductionRate;
                public float fAdsRecoilReductionLimit;
                public float fAdsRecoilReturnRate;
                public float fAdsViewScatterMin;
                public float fAdsViewScatterMax;
                public float fAdsSpread;
                public int hipGunKickReducedKickBullets;
                public float hipGunKickReducedKickPercent;
                public float fHipGunKickPitchMin;
                public float fHipGunKickPitchMax;
                public float fHipGunKickYawMin;
                public float fHipGunKickYawMax;
                public float fHipGunKickAccel;
                public float fHipGunKickSpeedMax;
                public float fHipGunKickSpeedDecay;
                public float fHipGunKickStaticDecay;
                public float fHipViewKickPitchMin;
                public float fHipViewKickPitchMax;
                public float fHipViewKickMinMagnitude;
                public float fHipViewKickYawMin;
                public float fHipViewKickYawMax;
                public float fHipViewScatterMin;
                public float fHipViewScatterMax;
                public float fAdsViewKickCenterDuckedScale;
                public float fAdsViewKickCenterProneScale;
                public float fAntiQuickScopeTime;
                public float fAntiQuickScopeScale;
                public float fAntiQuickScopeSpreadMultiplier;
                public float fAntiQuickScopeSpreadMax;
                public float fAntiQuickScopeSwayFactor;
                public float fightDist;
                public float maxDist;
                public fixed Pointer accuracyGraphName[2]; // char *
                public fixed Pointer accuracyGraphKnots[2]; // vec2_t *
                public fixed Pointer originalAccuracyGraphKnots[2]; // vec2_t *
                public fixed int accuracyGraphKnotCount[2];
                public fixed int originalAccuracyGraphKnotCount[2];
                public int iPositionReloadTransTime;
                public float leftArc;
                public float rightArc;
                public float topArc;
                public float bottomArc;
                public float accuracy;
                public float aiSpread;
                public float playerSpread;
                public fixed float minTurnSpeed[2];
                public fixed float maxTurnSpeed[2];
                public float pitchConvergenceTime;
                public float yawConvergenceTime;
                public float suppressTime;
                public float maxRange;
                public float fAnimHorRotateInc;
                public float fPlayerPositionDist;
                public Pointer szUseHintString; // char *
                public Pointer dropHintString; // char *
                public int iUseHintStringIndex;
                public int dropHintStringIndex;
                public float horizViewJitter;
                public float vertViewJitter;
                public float cameraShakeScale;
                public int cameraShakeDuration;
                public int cameraShakeRadius;
                public float explosionCameraShakeScale;
                public int explosionCameraShakeDuration;
                public int explosionCameraShakeRadius;
                public Pointer szScript; // char *
                public float destabilizationRateTime;
                public float destabilizationCurvatureMax;
                public int destabilizeDistance;
                public Pointer locationDamageMultipliers; // float *
                public Pointer fireRumble; // char *
                public Pointer meleeImpactRumble; // char *;
                public Pointer reloadRumble; // char *
                public Pointer explosionRumble; // char *
                public Pointer tracerType; // TracerDef *
                public Pointer enemyTracerType; // TracerDef *
                public float adsDofStart;
                public float adsDofEnd;
                public float hipDofStart;
                public float hipDofEnd;
                public float scanSpeed;
                public float scanAccel;
                public int scanPauseTime;
                public Pointer flameTableFirstPerson; // char *
                public Pointer flameTableThirdPerson; // char *
                public Pointer flameTableFirstPersonPtr; // flametable *
                public Pointer flameTableThirdPersonPtr; // flametable *
                public Pointer tagFx_preparationEffect; // FxEffectDef *
                public Pointer tagFlash_preparationEffect; // FxEffectDef *;
                [MarshalAs(UnmanagedType.U1)] public byte doGibbing;
                public float maxGibDistance;
                public float altScopeADSTransInTime;
                public float altScopeADSTransOutTime;
                public int iIntroFireTime;
                public int iIntroFireLength;
                public Pointer meleeSwipeEffect; // FxEffectDef *
                public Pointer meleeImpactEffect; // FxEffectDef *
                public Pointer meleeImpactNoBloodEffect; // FxEffectDef *
                public Pointer throwBackType; // char *
                public Pointer weaponCamo; // WeaponCamo *
                public float customFloat0;
                public float customFloat1;
                public float customFloat2;
                public int customBool0;
                public int customBool1;
                public int customBool2;
            };

            #endregion

            #region Tables

            private readonly static Tuple<string, int, int>[] WeaponOffsets =
            {
                new Tuple<string, int, int>("displayName", 0xC, 0x0),
                new Tuple<string, int, int>("AIOverlayDescription", 0x2CC, 0x0),
                new Tuple<string, int, int>("modeName", 0x2D8, 0x0),
                new Tuple<string, int, int>("playerAnimType", 0x2E4, 0x1b),
                new Tuple<string, int, int>("gunModel", 0xED4, 0xB),
                //new Tuple<string, int, int>("gunModel2", 0xED8, 0xB),
                //new Tuple<string, int, int>("gunModel3", 0xEDC, 0xB),
                //new Tuple<string, int, int>("gunModel4", 0xEE0, 0xB),
                //new Tuple<string, int, int>("gunModel5", 0xEE4, 0xB),
                //new Tuple<string, int, int>("gunModel6", 0xEE8, 0xB),
                //new Tuple<string, int, int>("gunModel7", 0xEEC, 0xB),
                //new Tuple<string, int, int>("gunModel8", 0xEF0, 0xB),
                //new Tuple<string, int, int>("gunModel9", 0xEF4, 0xB),
                //new Tuple<string, int, int>("gunModel10", 0xEF8, 0xB),
                //new Tuple<string, int, int>("gunModel11", 0xEFC, 0xB),
                //new Tuple<string, int, int>("gunModel12", 0xF00, 0xB),
                //new Tuple<string, int, int>("gunModel13", 0xF04, 0xB),
                //new Tuple<string, int, int>("gunModel14", 0xF08, 0xB),
                //new Tuple<string, int, int>("gunModel15", 0xF0C, 0xB),
                //new Tuple<string, int, int>("gunModel16", 0xF10, 0xB),
                new Tuple<string, int, int>("handModel", 0x2D4, 0xB),
                new Tuple<string, int, int>("hideTags", 0x1074, 0x2C),
                //new Tuple<string, int, int>("notetrackSoundMap", 0x10B4, 0x2E),
                new Tuple<string, int, int>("idleAnim", 0xF18, 0x0),
                new Tuple<string, int, int>("idleAnimLeft", 0x1058, 0x0),
                new Tuple<string, int, int>("emptyIdleAnim", 0xF1C, 0x0),
                new Tuple<string, int, int>("emptyIdleAnimLeft", 0x105C, 0x0),
                new Tuple<string, int, int>("fireIntroAnim", 0xF20, 0x0),
                new Tuple<string, int, int>("fireAnim", 0xF24, 0x0),
                new Tuple<string, int, int>("fireAnimLeft", 0x104C, 0x0),
                new Tuple<string, int, int>("holdFireAnim", 0xF28, 0x0),
                new Tuple<string, int, int>("lastShotAnim", 0xF2C, 0x0),
                new Tuple<string, int, int>("lastShotAnimLeft", 0x1050, 0x0),
                new Tuple<string, int, int>("flourishAnim", 0xF30, 0x0),
                new Tuple<string, int, int>("flourishAnimLeft", 0x1054, 0x0),
                new Tuple<string, int, int>("detonateAnim", 0xFFC, 0x0),
                new Tuple<string, int, int>("rechamberAnim", 0xF34, 0x0),
                new Tuple<string, int, int>("meleeAnim", 0xF38, 0x0),
                new Tuple<string, int, int>("meleeAnimEmpty", 0xF48, 0x0),
                new Tuple<string, int, int>("meleeAnim1", 0xF3C, 0x0),
                new Tuple<string, int, int>("meleeAnim2", 0xF40, 0x0),
                new Tuple<string, int, int>("meleeAnim3", 0xF44, 0x0),
                new Tuple<string, int, int>("meleeChargeAnim", 0xF4C, 0x0),
                new Tuple<string, int, int>("meleeChargeAnimEmpty", 0xF50, 0x0),
                new Tuple<string, int, int>("reloadAnim", 0xF54, 0x0),
                new Tuple<string, int, int>("reloadAnimRight", 0xF58, 0x0),
                new Tuple<string, int, int>("reloadAnimLeft", 0x1064, 0x0),
                new Tuple<string, int, int>("reloadEmptyAnim", 0xF5C, 0x0),
                new Tuple<string, int, int>("reloadEmptyAnimLeft", 0x1060, 0x0),
                new Tuple<string, int, int>("reloadStartAnim", 0xF60, 0x0),
                new Tuple<string, int, int>("reloadEndAnim", 0xF64, 0x0),
                new Tuple<string, int, int>("reloadQuickAnim", 0xF68, 0x0),
                new Tuple<string, int, int>("reloadQuickEmptyAnim", 0xF6C, 0x0),
                new Tuple<string, int, int>("raiseAnim", 0xF70, 0x0),
                new Tuple<string, int, int>("dropAnim", 0xF78, 0x0),
                new Tuple<string, int, int>("firstRaiseAnim", 0xF74, 0x0),
                new Tuple<string, int, int>("altRaiseAnim", 0xF7C, 0x0),
                new Tuple<string, int, int>("altDropAnim", 0xF80, 0x0),
                new Tuple<string, int, int>("quickRaiseAnim", 0xF84, 0x0),
                new Tuple<string, int, int>("quickDropAnim", 0xF88, 0x0),
                new Tuple<string, int, int>("emptyRaiseAnim", 0xF8C, 0x0),
                new Tuple<string, int, int>("emptyDropAnim", 0xF90, 0x0),
                new Tuple<string, int, int>("sprintInAnim", 0xF94, 0x0),
                new Tuple<string, int, int>("sprintLoopAnim", 0xF98, 0x0),
                new Tuple<string, int, int>("sprintOutAnim", 0xF9C, 0x0),
                new Tuple<string, int, int>("sprintInEmptyAnim", 0xFA0, 0x0),
                new Tuple<string, int, int>("sprintLoopEmptyAnim", 0xFA4, 0x0),
                new Tuple<string, int, int>("sprintOutEmptyAnim", 0xFA8, 0x0),
                new Tuple<string, int, int>("lowReadyInAnim", 0xFAC, 0x0),
                new Tuple<string, int, int>("lowReadyLoopAnim", 0xFB0, 0x0),
                new Tuple<string, int, int>("lowReadyOutAnim", 0xFB4, 0x0),
                new Tuple<string, int, int>("contFireInAnim", 0xFB8, 0x0),
                new Tuple<string, int, int>("contFireLoopAnim", 0xFBC, 0x0),
                new Tuple<string, int, int>("contFireOutAnim", 0xFC0, 0x0),
                new Tuple<string, int, int>("crawlInAnim", 0xFC4, 0x0),
                new Tuple<string, int, int>("crawlForwardAnim", 0xFC8, 0x0),
                new Tuple<string, int, int>("crawlBackAnim", 0xFCC, 0x0),
                new Tuple<string, int, int>("crawlRightAnim", 0xFD0, 0x0),
                new Tuple<string, int, int>("crawlLeftAnim", 0xFD4, 0x0),
                new Tuple<string, int, int>("crawlOutAnim", 0xFD8, 0x0),
                new Tuple<string, int, int>("crawlEmptyInAnim", 0xFDC, 0x0),
                new Tuple<string, int, int>("crawlEmptyForwardAnim", 0xFE0, 0x0),
                new Tuple<string, int, int>("crawlEmptyBackAnim", 0xFE4, 0x0),
                new Tuple<string, int, int>("crawlEmptyRightAnim", 0xFE8, 0x0),
                new Tuple<string, int, int>("crawlEmptyLeftAnim", 0xFEC, 0x0),
                new Tuple<string, int, int>("crawlEmptyOutAnim", 0xFF0, 0x0),
                new Tuple<string, int, int>("deployAnim", 0xFF4, 0x0),
                new Tuple<string, int, int>("breakdownAnim", 0xFF8, 0x0),
                new Tuple<string, int, int>("nightVisionWearAnim", 0x1000, 0x0),
                new Tuple<string, int, int>("nightVisionRemoveAnim", 0x1004, 0x0),
                new Tuple<string, int, int>("adsFireAnim", 0x1008, 0x0),
                new Tuple<string, int, int>("adsLastShotAnim", 0x100C, 0x0),
                new Tuple<string, int, int>("adsRechamberAnim", 0x1014, 0x0),
                new Tuple<string, int, int>("adsUpAnim", 0x1068, 0x0),
                new Tuple<string, int, int>("adsDownAnim", 0x106C, 0x0),
                new Tuple<string, int, int>("adsUpOtherScopeAnim", 0x1070, 0x0),
                new Tuple<string, int, int>("adsFireIntroAnim", 0x1010, 0x0),
                new Tuple<string, int, int>("deployAnim", 0xFF4, 0x0),
                new Tuple<string, int, int>("breakdownAnim", 0xFF8, 0x0),
                new Tuple<string, int, int>("dtp_in", 0x1018, 0x0),
                new Tuple<string, int, int>("dtp_loop", 0x101C, 0x0),
                new Tuple<string, int, int>("dtp_out", 0x1020, 0x0),
                new Tuple<string, int, int>("dtp_empty_in", 0x1024, 0x0),
                new Tuple<string, int, int>("dtp_empty_loop", 0x1028, 0x0),
                new Tuple<string, int, int>("dtp_empty_out", 0x102C, 0x0),
                new Tuple<string, int, int>("slide_in", 0x1030, 0x0),
                new Tuple<string, int, int>("mantleAnim", 0x1034, 0x0),
                new Tuple<string, int, int>("sprintCameraAnim", 0x1038, 0x0),
                new Tuple<string, int, int>("dtpInCameraAnim", 0x103C, 0x0),
                new Tuple<string, int, int>("dtpLoopCameraAnim", 0x1040, 0x0),
                new Tuple<string, int, int>("dtpOutCameraAnim", 0x1044, 0x0),
                new Tuple<string, int, int>("mantleCameraAnim", 0x1048, 0x0),
                new Tuple<string, int, int>("script", 0xBB8, 0x0),
                new Tuple<string, int, int>("weaponType", 0x2E8, 0x12),
                new Tuple<string, int, int>("weaponClass", 0x2EC, 0x13),
                new Tuple<string, int, int>("penetrateType", 0x2F0, 0x15),
                new Tuple<string, int, int>("impactType", 0x2F4, 0x16),
                new Tuple<string, int, int>("inventoryType", 0x2F8, 0x22),
                new Tuple<string, int, int>("fireType", 0x2FC, 0x23),
                new Tuple<string, int, int>("clipType", 0x300, 0x24),
                new Tuple<string, int, int>("barrelType", 0x304, 0x2B),
                new Tuple<string, int, int>("offhandClass", 0x32C, 0x19),
                new Tuple<string, int, int>("offhandSlot", 0x330, 0x1A),
                //last = 88 len
            };

            private readonly static string[] szWeapTypeNames =
            {
                "bullet",
                "grenade",
                "projectile",
                "binoculars",
                "gas",
                "bomb",
                "mine",
                "melee",
                "riotshield",
            };

            private readonly static string[] szWeapClassNames =
            {
                "rifle",
                "mg",
                "smg",
                "spread",
                "pistol",
                "grenade",
                "rocketlauncher",
                "turret",
                "non-player",
                "gas",
                "item",
                "melee",
                "Killstreak Alt Stored Weapon",
                "pistol spread",
            };

            private readonly static string[] szWeapInventoryTypeNames =
            {
                "primary",
                "offhand",
                "item",
                "altmode",
                "melee",
                "dwlefthand",
            };

            private readonly static string[] szWeapFireTypeNames =
            {
                "Full Auto",
                "Single Shot",
                "2-Round_Burst",
                "3-Round Burst",
                "4-Round Burst",
                "5-Round Burst",
                "Stacked Fire",
                "Minigun",
                "Charge Shot",
                "Jetgun",
            };

            private readonly static string[] szWeapClipTypeNames =
            {
                "bottom",
                "top",
                "left",
                "dp28",
                "ptrs",
                "lmg",
            };

            private readonly static string[] penetrateTypeNames =
            {
                "none",
                "small",
                "medium",
                "large",
            };

            private readonly static string[] impactTypeNames =
            {
                "none",
                "bullet_small",
                "bullet_large",
                "bullet_ap",
                "bullet_xtreme",
                "shotgun",
                "grenade_bounce",
                "grenade_explode",
                "rifle_grenade",
                "rocket_explode",
                "rocket_explode_xtreme",
                "projectile_dud",
                "mortar_shell",
                "tank_shell",
                "bolt",
                "blade",
            };

            private readonly static string[] szProjectileExplosionNames =
            {
                "grenade",
                "rocket",
                "flashbang",
                "none",
                "dud",
                "smoke",
                "heavy_explosive",
                "fire",
                "napalmblob",
                "bolt",
                "shrapnel_span",
            };

            private readonly static string[] szWeapOverlayReticleNames =
            {
                "none",
                "crosshair",
            };

            private readonly static string[] szWeapStanceNames =
            {
                "stand",
                "duck",
                "prone",
            };

            private readonly static string[] offhandClassNames =
            {
                "None",
                "Frag_Grenade",
                "Smoke_Grenade",
                "Flash_Grenade",
                "Gear",
                "Supply_Drop_Marker",
            };

            private readonly static string[] offhandSlotNames =
            {
                "None",
                "Lethal_grenade",
                "Tactical_grenade",
                "Equipment",
                "Specific_use",
            };

            private readonly static string[] activeReticleNames =
            {
                "None",
                "Pip-On-A-Stick",
                "Bouncing_Diamond",
                "Missile_Lock",

            };

            private readonly static string[] guidedMissileNames =
            {
                "None",
                "Sidewinder",
                "Hellfire",
                "Javelin",
                "Ballistic",
                "WireGuided",
                "TVGuided",
                "Drone",
                "HeatSeeking",
            };

            private readonly static string[] g_playerAnimTypeNames =
            {
                "none",
                "default",
                "other",
                "sniper",
                "club",
                "hold",
                "briefcase",
                "reviver",
                "radio",
                "dualwield",
                "remotecontrol",
                "minigun",
                "beltfed",
                "rearclip",
                "handleclip",
                "rearclipsniper",
                "ballisticknife",
                "singleknife",
                "nopump",
                "hatchet",
                "brawler",
                "zipline",
                "riotshield",
                "tablet",
                "turned",
                "sword",
                "groundslam",
                "armblade",
                "armminigun",
                "spikelauncher",
                "bow",
                "onehanded",
                "only_32_entries_work",
            };

            private readonly static string[] WeaponDamageTypes =
            {
               "normal",
               "normal_shotgun",
               "burned",
               "melee",
               "melee_assassinate",
               "melee_charge",
               "melee_bash",
               "melee_armblade",
               "explosive",
               "suicide",
               "falling",
               "blowback",
               "electrified",
               "annihilator",
               "fireflies",
               "bow_partial_charge",
               "bow_full_charge",
               "melee_bat",
               "energy_weapon",
            };

            private readonly static string[] stickinessNames =
            {
                "Don't stick",
                "Stick to all",
                "Stick to all, expect ai and clients",
                "Stick to ground",
                "Stick to ground, maintain yaw",
                "Stick to flesh",
            };

            private readonly static string[] rotateTypeNames =
            {
                "Rotate both axis, grenade style",
                "Rotate one axis, blade style",
                "Rotate like a cylinder",
            };

            private readonly static string[] overlayInterfaceNames =
            {
                "None",
                "Javelin",
                "Turrert Scope",
            };

            private readonly static string[] ammoCounterClipNames =
            {
                "None",
                "Magazine",
                "ShortMagazine",
                "Shotgun",
                "Rocket",
                "Beltfed",
                "AltWeapon",
            };

            private readonly static string[] barrelTypeNames =
            {
                "Single",
                "Dual Barrel",
                "Dual Barrel Alternate",
                "Quad Barrel",
                "Quad Barrel Alternate",
                "Quad Barrel Double Alternate",
            };

            #endregion

            public string Name => "weapon";

            public string SettingGroup => "weapon";

            public int Index => (int)AssetPool.weapon;

            public int AssetSize { get; set; }
            public int AssetCount { get; set; }
            public long StartAddress { get; set; }
            public long EndAddress { get { return StartAddress + (AssetCount * AssetSize); } set => throw new NotImplementedException(); }

            public List<Asset> Load(HydraInstance instance)
            {
                List<Asset> results = new List<Asset>();

                StartAddress = instance.Reader.ReadStruct<uint>(instance.Game.AssetPoolsAddress + (Index * 4));
                AssetCount = instance.Reader.ReadStruct<int>(instance.Game.AssetSizesAddress + (Index * 4));
                AssetSize = Marshal.SizeOf<WeaponVariantDef>();

                for (int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<WeaponVariantDef>(StartAddress + (i * AssetSize) + 4);

                    if (IsNullAsset(header.szInternalName))
                        continue;

                    var address = StartAddress + (i * AssetSize) + 4;

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.szInternalName),
                        Type = Name,
                        Status = "Loaded",
                        Data = address,
                        LoadMethod = ExportAsset,
                        Zone = "",
                        Information = "N/A"
                    });
                }

                return results;
            }

            private readonly static Dictionary<string, int> AnimTable = new Dictionary<string, int>
            {
                // 1AC == Start of Address
                ["idleAnim"] = 0x1AC,
                ["idleAnimLeft"] = 0x2EC,
                ["emptyIdleAnim"] = 0x1B0,
                ["emptyIdleAnimLeft"] = 0x2F0,
                ["fireIntroAnim"] = 0x1B4,
                ["fireAnim"] = 0x1B8,
                ["fireAnimLeft"] = 0x2E0,
                ["holdFireAnim"] = 0x1BC,
                ["lastShotAnim"] = 0x1C0,
                ["lastShotAnimLeft"] = 0x2E4,
                //["flourishAnim"] = 0x1C4,
                //["flourishAnimLeft"] = 0x2E8,
                ["detonateAnim"] = 0x290,
                ["rechamberAnim"] = 0x1C8,
                ["meleeAnim"] = 0x1CC,
                ["meleeAnimEmpty"] = 0x1DC,
                //["meleeAnim1"] = 0x1D0,
                //["meleeAnim2"] = 0x1D4,
                //["meleeAnim3"] = 0x1D8,
                ["meleeChargeAnim"] = 0x1E0,
                //["meleeChargeAnimEmpty"] = 0x1E4,
                ["reloadAnim"] = 0x1E8,
                //["reloadAnimRight"] = 0x1EC,
                ["reloadAnimLeft"] = 0x2F8,
                ["reloadEmptyAnim"] = 0x1F0,
                ["reloadEmptyAnimLeft"] = 0x2F4,
                ["reloadStartAnim"] = 0x1F4,
                ["reloadEndAnim"] = 0x1F8,
                ["reloadQuickAnim"] = 0x1FC,
                ["reloadQuickEmptyAnim"] = 0x200,
                ["raiseAnim"] = 0x204,
                ["dropAnim"] = 0x20C,
                ["firstRaiseAnim"] = 0x208,
                ["altRaiseAnim"] = 0x210,
                ["altDropAnim"] = 0x214,
                ["quickRaiseAnim"] = 0x218,
                ["quickDropAnim"] = 0x21C,
                ["emptyRaiseAnim"] = 0x220,
                ["emptyDropAnim"] = 0x224,
                ["sprintInAnim"] = 0x228,
                ["sprintLoopAnim"] = 0x22C,
                ["sprintOutAnim"] = 0x230,
                ["sprintInEmptyAnim"] = 0x234,
                ["sprintLoopEmptyAnim"] = 0x238,
                ["sprintOutEmptyAnim"] = 0x23C,
                ["lowReadyInAnim"] = 0x240,
                ["lowReadyLoopAnim"] = 0x244,
                ["lowReadyOutAnim"] = 0x248,
                ["contFireInAnim"] = 0x24C,
                ["contFireLoopAnim"] = 0x250,
                ["contFireOutAnim"] = 0x254,
                ["crawlInAnim"] = 0x258,
                ["crawlForwardAnim"] = 0x25C,
                ["crawlBackAnim"] = 0x260,
                ["crawlRightAnim"] = 0x264,
                ["crawlLeftAnim"] = 0x268,
                ["crawlOutAnim"] = 0x26C,
                ["crawlEmptyInAnim"] = 0x270,
                ["crawlEmptyForwardAnim"] = 0x274,
                ["crawlEmptyBackAnim"] = 0x278,
                ["crawlEmptyRightAnim"] = 0x27C,
                ["crawlEmptyLeftAnim"] = 0x280,
                ["crawlEmptyOutAnim"] = 0x284,
                //["deployAnim"] = 0x288,
                //["breakdownAnim"] = 0x28C,
                //["nightVisionWearAnim"] = 0x294,
                //["nightVisionRemoveAnim"] = 0x298,
                ["adsFireAnim"] = 0x29C,
                ["adsLastShotAnim"] = 0x2A0,
                ["adsRechamberAnim"] = 0x2A8,
                ["adsUpAnim"] = 0x2FC,
                ["adsDownAnim"] = 0x300,
                ["adsUpOtherScopeAnim"] = 0x304,
                ["adsFireIntroAnim"] = 0x2A4,
                ["slide_in"] = 0x2C4,
                //["deployAnim"] = 0x288,
                //["breakdownAnim"] = 0x28C,
                //["dtp_in"] = 0x2AC,
                //["dtp_loop"] = 0x2B0,
                //["dtp_out"] = 0x2B4,
                //["dtp_empty_in"] = 0x2B8,
                //["dtp_empty_loop"] = 0x2BC,
                //["dtp_empty_out"] = 0x2C0,
                //["slide_in"] = 0x2C4,
                //["mantleAnim"] = 0x2C8,
                //["sprintCameraAnim"] = 0x2CC,
                //["dtpInCameraAnim"] = 0x2D0,
                //["dtpLoopCameraAnim"] = 0x2D4,
                //["dtpOutCameraAnim"] = 0x2D8,
                //["mantleCameraAnim"] = 0x2DC,
            };

            public unsafe void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<WeaponVariantDef>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.szInternalName))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var weaponDef = instance.Reader.ReadStruct<WeaponDef>(header.weapDef);
                var gdtAsset = new GameDataTable.Asset(asset.Name, "bulletweapon");

                #region Misc

                gdtAsset["displayName"] = ReadString(header.szDisplayName, instance);
                gdtAsset["modeName"] = ReadString(weaponDef.szModeName, instance);
                gdtAsset["parentWeaponName"] = ReadString(weaponDef.parentWeaponName, instance);

                gdtAsset["playerAnimType"] = g_playerAnimTypeNames[weaponDef.playerAnimType];
                gdtAsset["DualWieldWeapon"] = ReadString(weaponDef.szDualWieldWeaponName, instance);
                gdtAsset["AIOverlayDescription"] = ReadString(weaponDef.szOverlayName, instance);

                gdtAsset["inventoryType"] = szWeapInventoryTypeNames[(int)weaponDef.inventoryType];
                gdtAsset["weaponType"] = szWeapTypeNames[(int)weaponDef.weapType];
                gdtAsset["weaponClass"] = szWeapClassNames[(int)weaponDef.weapClass];
                gdtAsset["pentrateType"] = penetrateTypeNames[(int)weaponDef.penetrateType];
                gdtAsset["impactType"] = impactTypeNames[(int)weaponDef.impactType];
                gdtAsset["offhandSlot"] = offhandSlotNames[(int)weaponDef.offhandSlot];
                gdtAsset["offhandClass"] = offhandSlotNames[(int)weaponDef.offhandClass];
                gdtAsset["fireType"] = szWeapFireTypeNames[(int)weaponDef.fireType];
                gdtAsset["clipType"] = szWeapClipTypeNames[(int)weaponDef.clipType];

                #endregion

                #region Ammo Options

                gdtAsset["rifleBullet"] = weaponDef.bRifleBullet;
                gdtAsset["armorPiercing"] = weaponDef.armorPiercing;
                gdtAsset["doGibbing"] = weaponDef.doGibbing;
                gdtAsset["damageType"] = WeaponDamageTypes[weaponDef.iDamageType];
                gdtAsset["maxGibDistance"] = weaponDef.maxGibDistance;

                #endregion

                #region Type Options

                gdtAsset["shotsBeforeRechamber"] = weaponDef.iShotsBeforeRechamber;
                gdtAsset["dualWield"] = weaponDef.bDualWield;
                gdtAsset["continuousFire"] = weaponDef.bContinuousFire;
                gdtAsset["isCarriedKillstreakWeapon"] = weaponDef.bIsCarriedKillstreakWeapon;
                gdtAsset["tvguided"] = header.bTVGuided;

                #endregion

                #region Alt Weapon Options

                gdtAsset["altWeapon"] = ReadString(header.szAltWeaponName, instance);
                gdtAsset["useAltTagFlash"] = weaponDef.bUseAltTagFlash;
                gdtAsset["altWeaponAdsOnly"] = weaponDef.bAltWeaponAdsOnly;
                gdtAsset["altWeaponDisableSwitching"] = weaponDef.bAltWeaponDisableSwitching;
                gdtAsset["ignoreAttachments"] = header.bIgnoreAttachments;

                #endregion

                #region Melee Fields

                gdtAsset["useAsMelee"] = weaponDef.bUseAsMelee;
                gdtAsset["meleeChargeRange"] = weaponDef.meleeChargeRange;
                gdtAsset["meleeTime"] = ConvertMStoSeconds(weaponDef.iMeleeTime);
                gdtAsset["meleeDelay"] = ConvertMStoSeconds(weaponDef.iMeleeDelay);
                gdtAsset["meleeChargeTime"] = ConvertMStoSeconds(weaponDef.meleeChargeTime);
                gdtAsset["meleeChargeDelay"] = ConvertMStoSeconds(weaponDef.meleeChargeDelay);
                gdtAsset["meleeDamage"] = weaponDef.iMeleeDamage;

                #endregion

                #region Reload Options

                gdtAsset["noPartialReload"] = weaponDef.bNoPartialReload;
                gdtAsset["segmentedReload"] = weaponDef.bSegmentedReload;
                gdtAsset["rechamberWhileAds"] = weaponDef.bRechamberWhileAds;
                gdtAsset["noADSAutoReload"] = weaponDef.bNoADSAutoReload;

                #endregion

                #region ADS Options

                gdtAsset["aimDownSight"] = weaponDef.aimDownSight;
                gdtAsset["adsFire"] = weaponDef.adsFireOnly;
                gdtAsset["noAdsWhenMagEmpty"] = weaponDef.noAdsWhenMagEmpty;
                gdtAsset["keepCrosshairWhenADS"] = weaponDef.bKeepCrosshairWhenADS;

                #endregion

                #region Micsellaneous Options

                gdtAsset["blocksProne"] = weaponDef.blocksProne;
                gdtAsset["avoidDropCleanup"] = weaponDef.avoidDropCleanup;
                gdtAsset["noDropsOrRaises"] = weaponDef.bNoDropsOrRaises;
                gdtAsset["noQuickDropWhenEmpty"] = weaponDef.bNoQuickDropWhenEmpty;
                gdtAsset["laserSight"] = weaponDef.laserSight;

                #endregion

                #region Attachment Perks

                gdtAsset["silenced"] = header.bSilenced;
                gdtAsset["DualMag"] = header.bDualMag;
                gdtAsset["infraRed"] = header.bInfraRed;

                #endregion

                #region Crosshair Options

                gdtAsset["enemyCrosshairRange"] = weaponDef.enemyCrosshairRange;
                gdtAsset["crosshairColorChange"] = weaponDef.crosshairColorChange;

                #endregion

                #region Lock On Options

                gdtAsset["requireLockonToFire"] = weaponDef.requireLockonToFire;
                gdtAsset["lockOnSpeed"] = weaponDef.lockOnSpeed;
                gdtAsset["lockOnRadius"] = weaponDef.lockOnRadius;

                #endregion

                #region Movement, Sprint, Turning

                gdtAsset["moveSpeedScale"] = weaponDef.moveSpeedScale;
                gdtAsset["adsMoveSpeedScale"] = weaponDef.adsMoveSpeedScale;
                gdtAsset["sprintDurationScale"] = weaponDef.sprintDurationScale;
                gdtAsset["gunMaxPitch"] = weaponDef.fGunMaxPitch;
                gdtAsset["gunMaxYaw"] = weaponDef.fGunMaxYaw;

                #endregion

                #region Aim Assist ( Console Only )

                gdtAsset["autoAimRange"] = weaponDef.autoAimRange;
                gdtAsset["aimAssistRange"] = weaponDef.aimAssistRange;

                #endregion

                #region Ammunition

                gdtAsset["sharedAmmo"] = weaponDef.sharedAmmo;
                gdtAsset["unlimitedAmmo"] = weaponDef.unlimitedAmmo;
                gdtAsset["ammoName"] = ReadString(header.szAmmoName, instance);
                gdtAsset["ammoDisplayName"] = ReadString(header.szAmmoDisplayName, instance);
                gdtAsset["ammoCountClipRelative"] = weaponDef.ammoCountClipRelative;
                gdtAsset["clipSize"] = header.iClipSize;
                gdtAsset["maxAmmo"] = weaponDef.iMaxAmmo;
                gdtAsset["startAmmo"] = weaponDef.iStartAmmo;
                gdtAsset["dropAmmoMin"] = weaponDef.iDropAmmoMin;
                gdtAsset["dropAmmoMax"] = weaponDef.iDropAmmoMax;
                gdtAsset["dropClipAmmoMin"] = weaponDef.iDropClipAmmoMin;
                gdtAsset["dropClipAmmoMax"] = weaponDef.iDropClipAmmoMax;
                gdtAsset["shotCount"] = weaponDef.shotCount;
                gdtAsset["reloadAmmoAdd"] = weaponDef.iReloadAmmoAdd;
                gdtAsset["reloadStartAdd"] = weaponDef.iReloadStartAdd;
                gdtAsset["lowAmmoWarningThreshold"] = weaponDef.lowAmmoWarningThreshold;
                gdtAsset["cancelAutoHolsterWhenEmpty"] = weaponDef.cancelAutoHolsterWhenEmpty;
                gdtAsset["suppressAmmoReserveDisplay"] = weaponDef.suppressAmmoReserveDisplay;
                gdtAsset["tankLifeTime"] = ConvertMStoSeconds(weaponDef.iTankLifeTime);

                #endregion

                #region Overheating

                gdtAsset["overheatWeapon"] = weaponDef.overheatWeapon;
                gdtAsset["coolWhileFiring"] = weaponDef.coolWhileFiring;
                gdtAsset["overheatRate"] = weaponDef.overheatRate;
                gdtAsset["overheatRate"] = weaponDef.cooldownRate;
                gdtAsset["overheatEndVal"] = weaponDef.overheatEndVal;

                #endregion

                #region Tracer

                gdtAsset["tracerType"] = instance.Game.GetAssetName(weaponDef.tracerType, instance);
                gdtAsset["enemyTracerType"] = instance.Game.GetAssetName(weaponDef.enemyTracerType, instance);

                #endregion

                #region Damage Ranges

                gdtAsset["damage"] = weaponDef.damage[0];
                gdtAsset["maxDamageRange"] = weaponDef.damageRange[0];
                gdtAsset["damage2"] = weaponDef.damage[1];
                gdtAsset["damageRange2"] = weaponDef.damageRange[1];
                gdtAsset["damage3"] = weaponDef.damage[2];
                gdtAsset["damageRange3"] = weaponDef.damageRange[2];
                gdtAsset["damage4"] = weaponDef.damage[3];
                gdtAsset["damageRange4"] = weaponDef.damageRange[3];
                gdtAsset["damage5"] = weaponDef.damage[4];
                gdtAsset["damageRange5"] = weaponDef.damageRange[4];
                gdtAsset["minDamage"] = weaponDef.damage[5];
                gdtAsset["minDamageRange"] = weaponDef.damageRange[5];

                #endregion

                #region Other Damage

                gdtAsset["playerDamage"] = weaponDef.playerDamage;
                gdtAsset["bulletImpactExplode"] = weaponDef.bBulletImpactExplode;
                gdtAsset["explosionRadius"] = weaponDef.iExplosionRadius;
                gdtAsset["explosionInnerDamage"] = weaponDef.iExplosionInnerDamage;
                gdtAsset["explosionOuterDamage"] = weaponDef.iExplosionOuterDamage;
                gdtAsset["damageConeAngle"] = weaponDef.damageConeAngle;

                #endregion

                #region Location Damage

                var locationDamageMultipliers = instance.Reader.ReadArray<float>(weaponDef.locationDamageMultipliers, 21);

                gdtAsset["locNone"] = locationDamageMultipliers[0];
                gdtAsset["locHelmet"] = locationDamageMultipliers[1];
                gdtAsset["locHead"] = locationDamageMultipliers[2];
                gdtAsset["locNect"] = locationDamageMultipliers[3];
                gdtAsset["locUpperTorso"] = locationDamageMultipliers[4];
                gdtAsset["locMidTorso"] = locationDamageMultipliers[5];
                gdtAsset["locLowerTorso"] = locationDamageMultipliers[6];
                gdtAsset["locLeftUpperArm"] = locationDamageMultipliers[7];
                gdtAsset["locRightUpperArm"] = locationDamageMultipliers[8];
                gdtAsset["locLeftLowerArm"] = locationDamageMultipliers[9];
                gdtAsset["locRightLowerArm"] = locationDamageMultipliers[10];
                gdtAsset["locLeftHand"] = locationDamageMultipliers[11];
                gdtAsset["locRightHand"] = locationDamageMultipliers[12];
                gdtAsset["locLeftUpperLeg"] = locationDamageMultipliers[13];
                gdtAsset["locRightUpperLeg"] = locationDamageMultipliers[14];
                gdtAsset["locLeftLowerLeg"] = locationDamageMultipliers[15];
                gdtAsset["locRightLowerLeg"] = locationDamageMultipliers[16];
                gdtAsset["locLeftFoot"] = locationDamageMultipliers[17];
                gdtAsset["locRightFoot"] = locationDamageMultipliers[18];
                gdtAsset["locGun"] = locationDamageMultipliers[19];

                #endregion

                #region State Timers

                gdtAsset["introFireTime"] = ConvertMStoSeconds(weaponDef.iIntroFireTime);
                gdtAsset["introFireLength"] = ConvertMStoSeconds(weaponDef.iIntroFireLength);
                gdtAsset["fireTime"] = ConvertMStoSeconds(weaponDef.iFireTime);
                gdtAsset["lastFireTime"] = ConvertMStoSeconds(weaponDef.iLastFireTime);
                gdtAsset["fireDelay"] = ConvertMStoSeconds(weaponDef.iFireDelay);
                gdtAsset["holdFireTime"] = ConvertMStoSeconds(weaponDef.iHoldFireTime);
                gdtAsset["burstDelayTime"] = ConvertMStoSeconds(weaponDef.iBurstDelayTime);
                gdtAsset["jamFireTime"] = ConvertMStoSeconds(weaponDef.iJamFireTime);
                gdtAsset["spinUpTime"] = ConvertMStoSeconds(weaponDef.iSpinUpTime);
                gdtAsset["spinDownTime"] = ConvertMStoSeconds(weaponDef.iSpinDownTime);
                gdtAsset["spinRate"] = weaponDef.spinRate;
                gdtAsset["reloadTime"] = ConvertMStoSeconds(header.iReloadTime);
                gdtAsset["reloadEmptyTime"] = ConvertMStoSeconds(header.iReloadEmptyTime);
                gdtAsset["reloadAddTime"] = ConvertMStoSeconds(weaponDef.iReloadAddTime);
                gdtAsset["reloadQuickTime"] = ConvertMStoSeconds(header.iReloadQuickTime);
                gdtAsset["reloadQuickEmptyTime"] = ConvertMStoSeconds(header.iReloadQuickEmptyTime);
                gdtAsset["reloadQuickAddTime"] = ConvertMStoSeconds(weaponDef.iReloadQuickAddTime);
                gdtAsset["reloadStartTime"] = ConvertMStoSeconds(weaponDef.iReloadStartTime);
                gdtAsset["reloadEndTime"] = ConvertMStoSeconds(weaponDef.iReloadEndTime);
                gdtAsset["reloadEmptyAddTime"] = ConvertMStoSeconds(weaponDef.iReloadEmptyAddTime);
                gdtAsset["reloadQuickEmptyAddTime"] = ConvertMStoSeconds(weaponDef.iReloadQuickEmptyAddTime);
                gdtAsset["rechamberTime"] = ConvertMStoSeconds(weaponDef.iRechamberTime);
                gdtAsset["rechamberBoltTime"] = ConvertMStoSeconds(weaponDef.iRechamberBoltTime);
                gdtAsset["dropTime"] = ConvertMStoSeconds(weaponDef.iDropTime);
                gdtAsset["raiseTime"] = ConvertMStoSeconds(weaponDef.iRaiseTime);
                gdtAsset["firstRaiseTime"] = ConvertMStoSeconds(weaponDef.iFirstRaiseTime);
                gdtAsset["altDropTime"] = ConvertMStoSeconds(weaponDef.iAltDropTime);
                gdtAsset["quickDropTime"] = ConvertMStoSeconds(weaponDef.quickDropTime);
                gdtAsset["quickRaiseTime"] = ConvertMStoSeconds(weaponDef.quickRaiseTime);
                gdtAsset["emptyDropTime"] = ConvertMStoSeconds(weaponDef.iEmptyDropTime);
                gdtAsset["emptyRaiseTime"] = ConvertMStoSeconds(weaponDef.iEmptyRaiseTime);
                gdtAsset["sprintInTime"] = ConvertMStoSeconds(weaponDef.sprintInTime);
                gdtAsset["sprintLoopTime"] = ConvertMStoSeconds(weaponDef.sprintLoopTime);
                gdtAsset["sprintOutTime"] = ConvertMStoSeconds(weaponDef.sprintOutTime);
                gdtAsset["lowReadyInTime"] = ConvertMStoSeconds(weaponDef.lowReadyInTime);
                gdtAsset["lowReadyLoopTime"] = ConvertMStoSeconds(weaponDef.lowReadyLoopTime);
                gdtAsset["lowReadyOutTime"] = ConvertMStoSeconds(weaponDef.lowReadyOutTime);
                gdtAsset["contFireInTime"] = ConvertMStoSeconds(weaponDef.contFireInTime);
                gdtAsset["contFireLoopTime"] = ConvertMStoSeconds(weaponDef.contFireLoopTime);
                gdtAsset["contFireOutTime"] = ConvertMStoSeconds(weaponDef.contFireOutTime);
                gdtAsset["crawlInTime"] = ConvertMStoSeconds(weaponDef.crawlInTime);
                gdtAsset["crawlForwardTime"] = ConvertMStoSeconds(weaponDef.crawlForwardTime);
                gdtAsset["crawlBackTime"] = ConvertMStoSeconds(weaponDef.crawlBackTime);
                gdtAsset["crawlRightTime"] = ConvertMStoSeconds(weaponDef.crawlRightTime);
                gdtAsset["crawlLeftTime"] = ConvertMStoSeconds(weaponDef.crawlLeftTime);
                gdtAsset["crawlOutFireTime"] = ConvertMStoSeconds(weaponDef.crawlOutFireTime);
                gdtAsset["crawlOutTime"] = ConvertMStoSeconds(weaponDef.crawlOutTime);
                gdtAsset["slideInTime"] = ConvertMStoSeconds(weaponDef.slideInTime);
                #endregion

                #region Sprint Movement Settings

                gdtAsset["sprintOfsF"] = weaponDef.vSprintOfs.X;
                gdtAsset["sprintOfsR"] = weaponDef.vSprintOfs.Y;
                gdtAsset["sprintOfsU"] = weaponDef.vSprintOfs.Z;

                gdtAsset["sprintRotP"] = weaponDef.vSprintRot.X;
                gdtAsset["sprintRotY"] = weaponDef.vSprintRot.Y;
                gdtAsset["sprintRotR"] = weaponDef.vSprintRot.Z;

                #endregion

                #region LowReady Movement Settings

                gdtAsset["lowReadyOfsF"] = weaponDef.vLowReadyOfs.X;
                gdtAsset["lowReadyOfsR"] = weaponDef.vLowReadyOfs.Y;
                gdtAsset["lowReadyOfsU"] = weaponDef.vLowReadyOfs.Z;

                gdtAsset["lowReadyRotP"] = weaponDef.vLowReadyRot.X;
                gdtAsset["lowReadyRotY"] = weaponDef.vLowReadyRot.Y;
                gdtAsset["lowReadyRotR"] = weaponDef.vLowReadyRot.Z;

                #endregion

                #region Riding Vehicle Settings

                gdtAsset["rideOfsF"] = weaponDef.vRideOfs.X;
                gdtAsset["rideOfsR"] = weaponDef.vRideOfs.Y;
                gdtAsset["rideOfsU"] = weaponDef.vRideOfs.Z;

                gdtAsset["rideRotP"] = weaponDef.vRideRot.X;
                gdtAsset["rideRotY"] = weaponDef.vRideRot.Y;
                gdtAsset["rideRotR"] = weaponDef.vRideRot.Z;

                #endregion

                #region Dive To Prone Movement Settings

                gdtAsset["dtpOfsF"] = weaponDef.vDtpOfs.X;
                gdtAsset["dtpOfsR"] = weaponDef.vDtpOfs.Y;
                gdtAsset["dtpOfsU"] = weaponDef.vDtpOfs.Z;

                gdtAsset["dtpRotP"] = weaponDef.vDtpRot.X;
                gdtAsset["dtpRotY"] = weaponDef.vDtpRot.Y;
                gdtAsset["dtpRotR"] = weaponDef.vDtpRot.Z;

                #endregion

                #region Mantle Movement Settings

                gdtAsset["mantleOfsF"] = weaponDef.vMantleOfs.X;
                gdtAsset["mantleOfsR"] = weaponDef.vMantleOfs.Y;
                gdtAsset["mantleOfsU"] = weaponDef.vMantleOfs.Z;

                gdtAsset["mantleRotP"] = weaponDef.vMantleRot.X;
                gdtAsset["mantleRotY"] = weaponDef.vMantleRot.Y;
                gdtAsset["mantleRotR"] = weaponDef.vMantleRot.Z;

                #endregion

                #region Player Slide Movement Settings

                gdtAsset["slideOfsF"] = weaponDef.vSlideOfs.X;
                gdtAsset["slideOfsR"] = weaponDef.vSlideOfs.Y;
                gdtAsset["slideOfsU"] = weaponDef.vSlideOfs.Z;

                gdtAsset["slideRotP"] = weaponDef.vSlideRot.X;
                gdtAsset["slideRotY"] = weaponDef.vSlideRot.Y;
                gdtAsset["slideRotR"] = weaponDef.vSlideRot.Z;

                #endregion

                #region Strafe Movement Settings

                gdtAsset["strafeMoveF"] = weaponDef.vStrafeMove.X;
                gdtAsset["strafeMoveR"] = weaponDef.vStrafeMove.Y;
                gdtAsset["strafeMoveU"] = weaponDef.vStrafeMove.Z;

                gdtAsset["strafeRotP"] = weaponDef.vStrafeRot.X;
                gdtAsset["strafeRotY"] = weaponDef.vStrafeRot.Y;
                gdtAsset["strafeRotR"] = weaponDef.vStrafeRot.Z;

                #endregion

                #region Stand Movement Settings

                gdtAsset["standMoveF"] = weaponDef.vStandMove.X;
                gdtAsset["standMoveR"] = weaponDef.vStandMove.Y;
                gdtAsset["standMoveU"] = weaponDef.vStandMove.Z;

                gdtAsset["standRotP"] = weaponDef.vStandRot.X;
                gdtAsset["standRotY"] = weaponDef.vStandRot.Y;
                gdtAsset["standRotR"] = weaponDef.vStandRot.Z;

                gdtAsset["standMoveMinSpeed"] = weaponDef.fStandMoveMinSpeed;
                gdtAsset["posMoveRate"] = weaponDef.fPosMoveRate;
                gdtAsset["standRotMinSpeed"] = weaponDef.fStandRotMinSpeed;
                gdtAsset["posRotRate"] = weaponDef.fPosRotRate;

                #endregion

                #region Crouch Movement Settings

                gdtAsset["duckedMoveF"] = weaponDef.vDuckedMove.X;
                gdtAsset["duckedMoveR"] = weaponDef.vDuckedMove.Y;
                gdtAsset["duckedMoveU"] = weaponDef.vDuckedMove.Z;

                gdtAsset["duckedRotP"] = weaponDef.vDuckedRot.X;
                gdtAsset["duckedRotY"] = weaponDef.vDuckedRot.Y;
                gdtAsset["duckedRotR"] = weaponDef.vDuckedRot.Z;

                gdtAsset["duckedOfsF"] = weaponDef.vDuckedOfs.X;
                gdtAsset["duckedOfsR"] = weaponDef.vDuckedOfs.Y;
                gdtAsset["duckedOfsU"] = weaponDef.vDuckedOfs.Z;

                gdtAsset["duckedMoveMinSpeed"] = weaponDef.fDuckedMoveMinSpeed;
                gdtAsset["duckedRotMinSpeed"] = weaponDef.fDuckedRotMinSpeed;

                #endregion

                #region Prone Movement Settings

                gdtAsset["proneMoveF"] = weaponDef.vProneMove.X;
                gdtAsset["proneMoveR"] = weaponDef.vProneMove.Y;
                gdtAsset["proneMoveU"] = weaponDef.vProneMove.Z;

                gdtAsset["proneRotP"] = weaponDef.vProneRot.X;
                gdtAsset["proneRotY"] = weaponDef.vProneRot.Y;
                gdtAsset["proneRotR"] = weaponDef.vProneRot.Z;

                gdtAsset["proneOfsF"] = weaponDef.vProneOfs.X;
                gdtAsset["proneOfsR"] = weaponDef.vProneOfs.Y;
                gdtAsset["proneOfsU"] = weaponDef.vProneOfs.Z;

                gdtAsset["proneMoveMinSpeed"] = weaponDef.fProneMoveMinSpeed;
                gdtAsset["proneRotMinSpeed"] = weaponDef.fProneRotMinSpeed;

                #endregion

                #region Idle Settings

                gdtAsset["hipIdleAmount"] = weaponDef.fHipIdleAmount;
                gdtAsset["adsIdleAmount"] = weaponDef.fAdsIdleAmount;
                gdtAsset["hipIdleSpeed"] = weaponDef.hipIdleSpeed;
                gdtAsset["adsIdleSpeed"] = weaponDef.adsIdleSpeed;
                gdtAsset["idleCrouchFactor"] = weaponDef.fIdleCrouchFactor;
                gdtAsset["idleProneFactor"] = weaponDef.fIdleProneFactor;

                #endregion

                #region ADS Settings
                // TO-DO: YOU FELL OFF L + RATIO!!!

                gdtAsset["adsSpread"] = weaponDef.fAdsSpread;
                gdtAsset["adsAimPitch"] = weaponDef.fAdsAimPitch;
                gdtAsset["adsTransInTime"] = ConvertMStoSeconds(header.iAdsTransInTime);
                gdtAsset["adsTransOutTime"] = ConvertMStoSeconds(header.iAdsTransOutTime);
                gdtAsset["adsCrosshairInFrac"] = weaponDef.fAdsCrosshairInFrac;
                gdtAsset["adsCrosshairOutFrac"] = weaponDef.fAdsCrosshairOutFrac;
                gdtAsset["adsZoom1_focalLength"] = header.fAdsZoomFov1;
                gdtAsset["adsZoom2_focalLength"] = header.fAdsZoomFov2;
                gdtAsset["adsZoom3_focalLength"] = header.fAdsZoomFov3;

                #endregion

                #region Hip Spread Settings

                gdtAsset["hipSpreadStandMin"] = weaponDef.fHipSpreadStandMin;
                gdtAsset["hipSpreadStandMax"] = weaponDef.hipSpreadStandMax;
                gdtAsset["hipSpreadDecayRate"] = weaponDef.fHipSpreadDecayRate;
                gdtAsset["hipSpreadDuckedMin"] = weaponDef.fHipSpreadDuckedMin;
                gdtAsset["hipSpreadDuckedMax"] = weaponDef.hipSpreadDuckedMax;
                gdtAsset["hipSpreadDuckedDecay"] = weaponDef.fHipSpreadDuckedDecay;
                gdtAsset["hipSpreadProneMin"] = weaponDef.fHipSpreadProneMin;
                gdtAsset["hipSpreadProneMax"] = weaponDef.hipSpreadProneMax;
                gdtAsset["hipSpreadProneDecay"] = weaponDef.fHipSpreadProneDecay;
                gdtAsset["hipSpreadFireAdd"] = weaponDef.fHipSpreadFireAdd;
                gdtAsset["hipSpreadMoveAdd"] = weaponDef.fHipSpreadMoveAdd;
                gdtAsset["hipSpreadTurnAdd"] = weaponDef.fHipSpreadTurnAdd;

                #endregion

                #region Gun Kick Settings

                gdtAsset["hipGunKickReducedKickBullets"] = weaponDef.hipGunKickReducedKickBullets;
                gdtAsset["adsGunKickReducedKickBullets"] = weaponDef.adsGunKickReducedKickBullets;
                gdtAsset["hipGunKickReducedKickPercent"] = weaponDef.hipGunKickReducedKickPercent;
                gdtAsset["adsGunKickReducedKickPercent"] = weaponDef.adsGunKickReducedKickPercent;
                gdtAsset["hipGunKickPitchMin"] = weaponDef.fHipGunKickPitchMin;
                gdtAsset["hipGunKickPitchMax"] = weaponDef.fHipGunKickPitchMax;
                gdtAsset["adsGunKickPitchMin"] = weaponDef.fAdsGunKickPitchMin;
                gdtAsset["adsGunKickPitchMax"] = weaponDef.fAdsGunKickPitchMax;
                gdtAsset["hipGunKickYawMin"] = weaponDef.fHipGunKickYawMin;
                gdtAsset["hipGunKickYawMax"] = weaponDef.fHipGunKickYawMax;
                gdtAsset["adsGunKickYawMin"] = weaponDef.fAdsGunKickYawMin;
                gdtAsset["adsGunKickYawMax"] = weaponDef.fAdsGunKickYawMax;
                gdtAsset["hipGunKickAccel"] = weaponDef.fHipGunKickAccel;
                gdtAsset["adsGunKickAccel"] = weaponDef.fAdsGunKickAccel;
                gdtAsset["hipGunKickSpeedMax"] = weaponDef.fHipGunKickSpeedMax;
                gdtAsset["adsGunKickSpeedMax"] = weaponDef.fAdsGunKickSpeedMax;
                gdtAsset["hipGunKickSpeedDecay"] = weaponDef.fHipGunKickSpeedDecay;
                gdtAsset["adsGunKickSpeedDecay"] = weaponDef.fAdsGunKickSpeedDecay;
                gdtAsset["hipGunKickStaticDecay"] = weaponDef.fHipGunKickStaticDecay;
                gdtAsset["adsGunKickStaticDecay"] = weaponDef.fAdsGunKickStaticDecay;

                #endregion

                #region View Kick Settings

                gdtAsset["hipViewKickPitchMin"] = weaponDef.fHipViewKickPitchMin;
                gdtAsset["hipViewKickPitchMax"] = weaponDef.fHipViewKickPitchMax;
                gdtAsset["adsViewKickPitchMin"] = weaponDef.fAdsViewKickPitchMin;
                gdtAsset["adsViewKickPitchMax"] = weaponDef.fAdsViewKickPitchMax;
                gdtAsset["hipViewKickYawMin"] = weaponDef.fHipViewKickYawMin;
                gdtAsset["hipViewKickYawMax"] = weaponDef.fHipViewKickYawMax;
                gdtAsset["adsViewKickYawMin"] = weaponDef.fAdsViewKickYawMin;
                gdtAsset["adsViewKickYawMax"] = weaponDef.fAdsViewKickYawMax;
                gdtAsset["hipViewKickMinMagnitude"] = weaponDef.fHipViewKickMinMagnitude;
                gdtAsset["adsViewKickMinMagnitude"] = weaponDef.fAdsViewKickMinMagnitude;
                gdtAsset["hipViewKickCenterSpeed"] = header.fHipViewKickCenterSpeed;
                gdtAsset["adsViewKickCenterSpeed"] = header.fAdsViewKickCenterSpeed;
                gdtAsset["adsRecoilReductionRate"] = weaponDef.fAdsRecoilReductionRate;
                gdtAsset["adsRecoilReductionLimit"] = weaponDef.fAdsRecoilReductionLimit;
                gdtAsset["adsRecoilReturnRate"] = weaponDef.fAdsRecoilReturnRate;

                #endregion

                #region Sway Settings

                gdtAsset["swayMaxAngle"] = weaponDef.swayMaxAngle;
                gdtAsset["adsSwayMaxAngle"] = weaponDef.adsSwayMaxAngle;
                gdtAsset["swayLerpSpeed"] = weaponDef.swayLerpSpeed;
                gdtAsset["adsSwayLerpSpeed"] = weaponDef.adsSwayLerpSpeed;
                gdtAsset["swayPitchScale"] = weaponDef.swayPitchScale;
                gdtAsset["adsSwayPitchScale"] = weaponDef.adsSwayPitchScale;
                gdtAsset["swayYawScale"] = weaponDef.swayYawScale;
                gdtAsset["adsSwayYawScale"] = weaponDef.adsSwayYawScale;
                gdtAsset["swayHorizScale"] = weaponDef.swayHorizScale;
                gdtAsset["adsSwayHorizScale"] = header.fAdsSwayHorizScale;
                gdtAsset["swayVertScale"] = weaponDef.swayVertScale;
                gdtAsset["adsSwayVertScale"] = header.fAdsSwayVertScale;

                #endregion

                #region Mountable Weaponary

                gdtAsset["mountableWeapon"] = weaponDef.mountableWeapon;
                gdtAsset["deployTime"] = weaponDef.deployTime / 1000.0d;
                gdtAsset["breakdownTime"] = weaponDef.breakdownTime / 1000.0d;
                gdtAsset["mountedModel"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.mountedModel), instance);

                #endregion

                #region AI Settings

                gdtAsset["fightDist"] = weaponDef.fightDist;
                gdtAsset["maxDist"] = weaponDef.maxDist;
                gdtAsset["aiSpread"] = weaponDef.aiSpread;

                #endregion

                #region Reticle Settings

                gdtAsset["reticleCenter"] = instance.Game.GetAssetName(weaponDef.reticleCenter, instance);
                gdtAsset["reticleCenterSize"] = weaponDef.iReticleCenterSize;
                gdtAsset["reticleSide"] = instance.Game.GetAssetName(weaponDef.reticleSide, instance);
                gdtAsset["reticleSideSize"] = weaponDef.iReticleSideSize;
                gdtAsset["hipReticleSidePos"] = weaponDef.fHipReticleSidePos;
                gdtAsset["reticleMinOfs"] = weaponDef.iReticleMinOfs;

                #endregion

                #region Anti Quick Scope Settings

                gdtAsset["antiQuickScope"] = header.bAntiQuickScope;
                gdtAsset["antiQuickScopeTime"] = weaponDef.fAntiQuickScopeTime;
                gdtAsset["antiQuickScopeScale"] = weaponDef.fAntiQuickScopeScale;
                gdtAsset["antiQuickScopeSpreadMultiplier"] = weaponDef.fAntiQuickScopeSpreadMultiplier;
                gdtAsset["antiQuickScopeSpreadMax"] = weaponDef.fAntiQuickScopeSpreadMax;
                gdtAsset["antiQuickScopeSwayFactor"] = weaponDef.fAntiQuickScopeSwayFactor;

                #endregion

                // ADS Overlay Settings doesn't exist

                #region XModels

                gdtAsset["gunModel"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.gunXModel), instance);
                gdtAsset["viewmodelTag"] = instance.Game.GetAssetName(header.attachViewModelTag, instance);
                gdtAsset["worldModel"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.worldModel), instance);
                gdtAsset["worldModelTagRight"] = instance.Game.GetAssetName(header.attachWorldModelTag, instance);
                gdtAsset["useDroppedModelAsStowed"] = weaponDef.bUseDroppedModelAsStowed;
                gdtAsset["stowedModelOffsetsF"] = header.stowedModelOffsets.X;
                gdtAsset["stowedModelOffsetsR"] = header.stowedModelOffsets.Y;
                gdtAsset["stowedModelOffsetsU"] = header.stowedModelOffsets.Z;
                gdtAsset["stowedModelOffsetsPitch"] = header.stowedModelRotations.X;
                gdtAsset["stowedModelOffsetsYaw"] = header.stowedModelRotations.Y;
                gdtAsset["stowedModelOffsetsRoll"] = header.stowedModelRotations.Z;
                gdtAsset["handModel"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.handXModel), instance);
                gdtAsset["worldClipModel"] = instance.Game.GetAssetName(weaponDef.worldClipModel, instance);
                //gdtAsset["hideTags"] = ReadString(header.hideTags, instance);

                #endregion

                // Attachments could be different from bo2 to bo3

                #region Camo

                gdtAsset["weaponCamo"] = ReadString(instance.Reader.ReadUInt32(weaponDef.weaponCamo), instance);

                #endregion

                #region XAnims

                foreach (var key in AnimTable.Keys)
                {
                    gdtAsset[key] = ReadString(instance.Reader.ReadUInt32((header.szXAnims) + (AnimTable[key] - 0x1A8)), instance);
                }

                #endregion

                // Melee is done in XAnims

                // Swimming doesn't exist

                #region FX

                gdtAsset["viewFlashEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.viewFlashEffect), instance);
                gdtAsset["viewFlashOffsetF"] = weaponDef.vViewFlashOffset.X;
                gdtAsset["viewFlashOffsetR"] = weaponDef.vViewFlashOffset.Y;
                gdtAsset["viewFlashOffsetU"] = weaponDef.vViewFlashOffset.Z;
                gdtAsset["worldFlashEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.worldFlashEffect), instance);
                gdtAsset["worldFlashOffsetF"] = weaponDef.vWorldFlashOffset.X;
                gdtAsset["worldFlashOffsetR"] = weaponDef.vWorldFlashOffset.Y;
                gdtAsset["worldFlashOffsetU"] = weaponDef.vWorldFlashOffset.Z;
                gdtAsset["viewShellEjectEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.viewShellEjectEffect), instance);
                gdtAsset["viewShellEjectOffsetF"] = weaponDef.vViewShellEjectOffset.X;
                gdtAsset["viewShellEjectOffsetR"] = weaponDef.vViewShellEjectOffset.Y;
                gdtAsset["viewShellEjectOffsetU"] = weaponDef.vViewShellEjectOffset.Z;
                gdtAsset["viewShellEjectRotationP"] = weaponDef.vViewShellEjectRotation.X;
                gdtAsset["viewShellEjectRotationY"] = weaponDef.vViewShellEjectRotation.Y;
                gdtAsset["viewShellEjectRotationR"] = weaponDef.vViewShellEjectRotation.Z;
                gdtAsset["worldShellEjectEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.worldShellEjectEffect), instance);
                gdtAsset["worldShellEjectOffsetF"] = weaponDef.vWorldShellEjectOffset.X;
                gdtAsset["worldShellEjectOffsetR"] = weaponDef.vWorldShellEjectOffset.Y;
                gdtAsset["worldShellEjectOffsetU"] = weaponDef.vWorldShellEjectOffset.Z;
                gdtAsset["worldShellEjectRotationP"] = weaponDef.vWorldShellEjectRotation.X;
                gdtAsset["worldShellEjectRotationY"] = weaponDef.vWorldShellEjectRotation.Y;
                gdtAsset["worldShellEjectRotationR"] = weaponDef.vWorldShellEjectRotation.Z;
                gdtAsset["viewLastShotEjectEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.viewLastShotEjectEffect), instance);
                gdtAsset["worldLastShotEjectEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.worldLastShotEjectEffect), instance);
                gdtAsset["barrelCooldownMinCount"] = weaponDef.barrelCooldownMinCount;
                gdtAsset["barrelCooldownEffect"] = instance.Game.GetAssetName(instance.Reader.ReadUInt32(weaponDef.barrelCooldownEffect), instance);

                #endregion

                // Impacts doesn't exist

                // Water properties doesn't exist

                #region Sounds

                gdtAsset["ammoPickupSound"] = ReadString(weaponDef.ammoPickupSound, instance);
                gdtAsset["ammoPickupSoundPlayer"] = ReadString(weaponDef.ammoPickupSoundPlayer, instance);
                gdtAsset["pullbackSound"] = ReadString(weaponDef.pullbackSound, instance);
                gdtAsset["pullbackSoundPlayer"] = ReadString(weaponDef.pullbackSoundPlayer, instance);
                gdtAsset["fireSound"] = ReadString(weaponDef.fireSound, instance);
                gdtAsset["fireSoundPlayer"] = ReadString(weaponDef.fireSoundPlayer, instance);
                gdtAsset["fireLastSound"] = ReadString(weaponDef.fireLastSound, instance);
                gdtAsset["fireLastSoundPlayer"] = ReadString(weaponDef.fireLastSoundPlayer, instance);
                gdtAsset["emptyFireSound"] = ReadString(weaponDef.emptyFireSound, instance);
                gdtAsset["emptyFireSoundPlayer"] = ReadString(weaponDef.emptyFireSoundPlayer, instance);
                gdtAsset["spinLoopSound"] = ReadString(weaponDef.spinLoopSound, instance);
                gdtAsset["spinLoopSoundPlayer"] = ReadString(weaponDef.spinLoopSoundPlayer, instance);
                gdtAsset["startSpinSound"] = ReadString(weaponDef.startSpinSound, instance);
                gdtAsset["startSpinSoundPlayer"] = ReadString(weaponDef.startSpinSoundPlayer, instance);
                gdtAsset["stopSpinSound"] = ReadString(weaponDef.stopSpinSound, instance);
                gdtAsset["stopSpinSoundPlayer"] = ReadString(weaponDef.stopSpinSoundPlayer, instance);
                gdtAsset["applySpinPitch"] = weaponDef.applySpinPitch;
                gdtAsset["startFireSound"] = ReadString(weaponDef.fireStartSound, instance);
                gdtAsset["startFireSoundPlayer"] = ReadString(weaponDef.fireStartSoundPlayer, instance);
                gdtAsset["killcamFireSound"] = ReadString(weaponDef.fireKillcamSound, instance);
                gdtAsset["killcamFireSoundPlayer"] = ReadString(weaponDef.fireKillcamSoundPlayer, instance);
                gdtAsset["loopFireSound"] = ReadString(weaponDef.fireLoopSound, instance);
                gdtAsset["loopFireSoundPlayer"] = ReadString(weaponDef.fireLoopSoundPlayer, instance);
                gdtAsset["loopFireEndSound"] = ReadString(weaponDef.fireLoopEndSound, instance);
                gdtAsset["loopFireEndSoundPlayer"] = ReadString(weaponDef.fireLoopEndSoundPlayer, instance);
                gdtAsset["crackSound"] = ReadString(weaponDef.crackSound, instance);
                gdtAsset["whizbySound"] = ReadString(weaponDef.whizbySound, instance);
                gdtAsset["whizbySound"] = ReadString(weaponDef.whizbySound, instance);
                gdtAsset["deploySound"] = ReadString(weaponDef.deploySound, instance);
                gdtAsset["deploySoundPlayer"] = ReadString(weaponDef.deploySoundPlayer, instance);
                gdtAsset["finishDeploySound"] = ReadString(weaponDef.finishDeploySound, instance);
                gdtAsset["finishDeploySoundPlayer"] = ReadString(weaponDef.finishDeploySoundPlayer, instance);
                gdtAsset["breakdownSound"] = ReadString(weaponDef.breakdownSound, instance);
                gdtAsset["breakdownSoundPlayer"] = ReadString(weaponDef.breakdownSoundPlayer, instance);
                gdtAsset["rechamberSound"] = ReadString(weaponDef.rechamberSound, instance);
                gdtAsset["rechamberSoundPlayer"] = ReadString(weaponDef.rechamberSoundPlayer, instance);
                gdtAsset["reloadSound"] = ReadString(weaponDef.reloadSound, instance);
                gdtAsset["reloadSoundPlayer"] = ReadString(weaponDef.reloadSoundPlayer, instance);
                gdtAsset["reloadEmptySound"] = ReadString(weaponDef.reloadEmptySound, instance);
                gdtAsset["reloadEmptySoundPlayer"] = ReadString(weaponDef.reloadEmptySoundPlayer, instance);
                gdtAsset["reloadStartSound"] = ReadString(weaponDef.reloadStartSound, instance);
                gdtAsset["reloadStartSoundPlayer"] = ReadString(weaponDef.reloadStartSoundPlayer, instance);
                gdtAsset["reloadEndSound"] = ReadString(weaponDef.reloadEndSound, instance);
                gdtAsset["reloadEndSoundPlayer"] = ReadString(weaponDef.reloadEndSoundPlayer, instance);
                gdtAsset["altSwitchSound"] = ReadString(weaponDef.altSwitchSound, instance);
                gdtAsset["altSwitchSoundPlayer"] = ReadString(weaponDef.altSwitchSoundPlayer, instance);
                gdtAsset["raiseSound"] = ReadString(weaponDef.raiseSound, instance);
                gdtAsset["raiseSoundPlayer"] = ReadString(weaponDef.raiseSoundPlayer, instance);
                gdtAsset["firstRaiseSound"] = ReadString(weaponDef.firstRaiseSound, instance);
                gdtAsset["firstRaiseSoundPlayer"] = ReadString(weaponDef.firstRaiseSoundPlayer, instance);
                gdtAsset["adsRaiseSoundPlayer"] = ReadString(weaponDef.adsRaiseSoundPlayer, instance);
                gdtAsset["adsLowerSoundPlayer"] = ReadString(weaponDef.adsLowerSoundPlayer, instance);
                gdtAsset["putawaySound"] = ReadString(weaponDef.putawaySound, instance);
                gdtAsset["putawaySoundPlayer"] = ReadString(weaponDef.putawaySoundPlayer, instance);
                gdtAsset["shellCasing"] = ReadString(weaponDef.shellCasing, instance);
                gdtAsset["shellCasingPlayer"] = ReadString(weaponDef.shellCasingPlayer, instance);

                #endregion

                // Crack Sound Settings doesn't exist

                #region Rumbles

                gdtAsset["fireRumble"] = ReadString(weaponDef.fireRumble, instance);
                gdtAsset["reloadRumble"] = ReadString(weaponDef.reloadRumble, instance);

                #endregion

                // Weapon Rest doesn't exist

                #region UI

                gdtAsset["hudIcon"] = instance.Game.GetAssetName(weaponDef.hudIcon, instance);
                gdtAsset["hudIconRatio"] = weapIconRatioNames[(int)weaponDef.hudIconRatio];
                gdtAsset["killIcon"] = instance.Game.GetAssetName(weaponDef.killIcon, instance);
                gdtAsset["killIconRatio"] = weapIconRatioNames[(int)weaponDef.killIconRatio];
                gdtAsset["dpadIcon"] = instance.Game.GetAssetName(header.dpadIcon, instance);
                gdtAsset["dpadIconRatio"] = weapIconRatioNames[(int)header.dpadIconRatio];
                gdtAsset["noAmmoOnDpadIcon"] = header.noAmmoOnDpadIcon;
                gdtAsset["ammoCounterIcon"] = instance.Game.GetAssetName(weaponDef.ammoCounterIcon, instance);
                gdtAsset["ammoCounterIconRatio"] = weapIconRatioNames[(int)weaponDef.ammoCounterIconRatio];
                // gdtAsset["ammoCounterClip"] = iconRatioTypes[(int)weaponDef.ammoCounterClip]; sex
                gdtAsset["fireTypeIcon"] = instance.Game.GetAssetName(weaponDef.fireTypeIcon, instance);

                #endregion

                #region DOF Settings

                gdtAsset["adsDofStart"] = weaponDef.adsDofStart;
                gdtAsset["adsDofEnd"] = weaponDef.adsDofEnd;

                #endregion

                gdtAsset["customBool0"] = weaponDef.customBool0;
                gdtAsset["customBool1"] = weaponDef.customBool1;
                gdtAsset["customBool2"] = weaponDef.customBool2;

                gdtAsset["customFloat0"] = weaponDef.customFloat0;
                gdtAsset["customFloat1"] = weaponDef.customFloat1;
                gdtAsset["customFloat2"] = weaponDef.customFloat2;

                gdtAsset.Name = asset.Name;
                instance.AddGDTAsset(gdtAsset, "bulletweapon", gdtAsset.Name);
            }

            public double ConvertMStoSeconds(int ms) => ms / 1000.0;

            public string ReadString(long position, HydraInstance instance)
            {
                return position != 0 ? instance.Reader.ReadNullTerminatedString(position) : "";
            }

            public bool IsNullAsset(long nameAddress)
            {
                return nameAddress >= StartAddress && nameAddress <= EndAddress || nameAddress == 0;
            }

            private static object BG_ParseWeaponDefSpecificFieldType(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch(type)
                {
                    case 0x12:
                        return szWeapTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x13:
                        return szWeapClassNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x14:
                        return szWeapOverlayReticleNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x15:
                        return penetrateTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x16:
                        return impactTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x17:
                        return szWeapStanceNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x18:
                        return szProjectileExplosionNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x19:
                        return offhandClassNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x1a:
                        return offhandSlotNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x1b:
                        return g_playerAnimTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x1c:
                        return activeReticleNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x1d:
                        return guidedMissileNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x1e:
                        return ""; // UNKNOWN
                    case 0x1f:
                        return stickinessNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x20:
                        return rotateTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x21:
                        return overlayInterfaceNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x22:
                        return szWeapInventoryTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x23:
                        return szWeapFireTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x24:
                        return szWeapClipTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x25:
                        return ammoCounterClipNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x26:
                        return weapIconRatioNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x27:
                        return weapIconRatioNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x28:
                        return weapIconRatioNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x29:
                        return weapIconRatioNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x2a:
                        return weapIconRatioNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x2b:
                        return barrelTypeNames[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x2c:
                        return ""; // UNKNOWN
                    case 0x2d:
                        return ""; // UNKNOWN
                    case 0x2e:
                        return ""; // UNKNOWN
                    case 0x2f:
                        return ""; // UNKNOWN
                    default:
                        return null;
                }
            }
        }
    }
}
