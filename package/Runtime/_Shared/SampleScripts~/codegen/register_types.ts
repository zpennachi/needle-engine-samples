﻿/* eslint-disable */
import { TypeStore } from "@needle-tools/engine"

// Import types
import { AspectRatioFitterUI } from "../AspectRatioFitterUI.js";
import { CameraGoal } from "../CameraGoal.js";
import { CameraLayer } from "../CameraLayer.js";
import { Cannon } from "../Cannon.js";
import { RandomColor } from "../ChangeColor.js";
import { ChangeColorOnCollision } from "../ChangeColorOnCollision.js";
import { IncreaseShaderSpeedOverTime } from "../ChangeCustomShaderProperty.js";
import { DebugLOD } from "../DebugLOD.js";
import { DepthOfFieldController } from "../DepthOfFieldController.js";
import { DisableEnvironmentLight } from "../DisableEnvironment.js";
import { EmitParticlesOnClick } from "../EmitParticlesOnClick.js";
import { EnforceParameters } from "../EnforceParameters.js";
import { HTMLButtonClick } from "../HTMLButtonEvent.js";
import { HTMLMenu } from "../HTMLMenu.js";
import { IFrameContent } from "../IFrameContent.js";
import { ImageTrackingDownloadUI } from "../ImageTrackingDownloadUI.js";
import { Networking_ClickToChangeColor } from "../Networking.js";
import { Networking_StringArray } from "../Networking.js";
import { Networking_Object } from "../Networking.js";
import { PerformanceSettings } from "../PerformanceSettings.js";
import { PhysicsCollision } from "../PhysicsCollision.js";
import { PhysicsTrigger } from "../PhysicsCollision.js";
import { PlayAnimationOnCollision } from "../PlayAnimationOnCollision.js";
import { PlayAnimationOnTrigger } from "../PlayAnimationOnTrigger.js";
import { PlayAudioOnCollision } from "../PlayAudioOnCollision.js";
import { ResponsiveContent } from "../ResponsiveContent.js";
import { LoadingSceneRoot } from "../SceneSwitcher.js";
import { SceneLoadingEvents } from "../SceneSwitcher.js";
import { SceneSwitcherControls } from "../SceneSwitcherControls.js";
import { ShowBalloonMessage } from "../ShowBalloonMessage.js";
import { SimpleLeaderboard } from "../SimpleLeaderboard.js";
import { StartPosition } from "../StartPosition.js";
import { AutoReset } from "../StartPosition.js";
import { SyncedRoomUI } from "../SyncedRoomUI.js";
import { TimedSpawn } from "../TimedSpawn.js";
import { VideoBackground } from "../VideoBackground.js";
import { ResetPositionOnInterval } from "../VisibilitySamples.js";
import { ToggleVisibility } from "../VisibilitySamples.js";
import { WireframeMaterial } from "../WireframeMaterial.js";
import { XRLifecycleEvents } from "../XRLifecycleEvents.js";
import { Arrow } from "../Bow & Arrow/Arrow.js";
import { ArrowShooting } from "../Bow & Arrow/ArrowShooting.js";
import { BowArrowTarget } from "../Bow & Arrow/BowArrowTarget.js";
import { BowTargetSpawner } from "../Bow & Arrow/BowTargetSpawner.js";
import { VariantInfo } from "../Variants/VariantInfo.js";
import { VariantSwitcher } from "../Variants/VariantSwitcher.js";

// Register types
TypeStore.add("AspectRatioFitterUI", AspectRatioFitterUI);
TypeStore.add("CameraGoal", CameraGoal);
TypeStore.add("CameraLayer", CameraLayer);
TypeStore.add("Cannon", Cannon);
TypeStore.add("RandomColor", RandomColor);
TypeStore.add("ChangeColorOnCollision", ChangeColorOnCollision);
TypeStore.add("IncreaseShaderSpeedOverTime", IncreaseShaderSpeedOverTime);
TypeStore.add("DebugLOD", DebugLOD);
TypeStore.add("DepthOfFieldController", DepthOfFieldController);
TypeStore.add("DisableEnvironmentLight", DisableEnvironmentLight);
TypeStore.add("EmitParticlesOnClick", EmitParticlesOnClick);
TypeStore.add("EnforceParameters", EnforceParameters);
TypeStore.add("HTMLButtonClick", HTMLButtonClick);
TypeStore.add("HTMLMenu", HTMLMenu);
TypeStore.add("IFrameContent", IFrameContent);
TypeStore.add("ImageTrackingDownloadUI", ImageTrackingDownloadUI);
TypeStore.add("Networking_ClickToChangeColor", Networking_ClickToChangeColor);
TypeStore.add("Networking_StringArray", Networking_StringArray);
TypeStore.add("Networking_Object", Networking_Object);
TypeStore.add("PerformanceSettings", PerformanceSettings);
TypeStore.add("PhysicsCollision", PhysicsCollision);
TypeStore.add("PhysicsTrigger", PhysicsTrigger);
TypeStore.add("PlayAnimationOnCollision", PlayAnimationOnCollision);
TypeStore.add("PlayAnimationOnTrigger", PlayAnimationOnTrigger);
TypeStore.add("PlayAudioOnCollision", PlayAudioOnCollision);
TypeStore.add("ResponsiveContent", ResponsiveContent);
TypeStore.add("LoadingSceneRoot", LoadingSceneRoot);
TypeStore.add("SceneLoadingEvents", SceneLoadingEvents);
TypeStore.add("SceneSwitcherControls", SceneSwitcherControls);
TypeStore.add("ShowBalloonMessage", ShowBalloonMessage);
TypeStore.add("SimpleLeaderboard", SimpleLeaderboard);
TypeStore.add("StartPosition", StartPosition);
TypeStore.add("AutoReset", AutoReset);
TypeStore.add("SyncedRoomUI", SyncedRoomUI);
TypeStore.add("TimedSpawn", TimedSpawn);
TypeStore.add("VideoBackground", VideoBackground);
TypeStore.add("ResetPositionOnInterval", ResetPositionOnInterval);
TypeStore.add("ToggleVisibility", ToggleVisibility);
TypeStore.add("WireframeMaterial", WireframeMaterial);
TypeStore.add("XRLifecycleEvents", XRLifecycleEvents);
TypeStore.add("Arrow", Arrow);
TypeStore.add("ArrowShooting", ArrowShooting);
TypeStore.add("BowArrowTarget", BowArrowTarget);
TypeStore.add("BowTargetSpawner", BowTargetSpawner);
TypeStore.add("VariantInfo", VariantInfo);
TypeStore.add("VariantSwitcher", VariantSwitcher);
