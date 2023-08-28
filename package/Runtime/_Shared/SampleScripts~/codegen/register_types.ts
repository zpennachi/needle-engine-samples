﻿import { TypeStore } from "@needle-tools/engine"

// Import types
import { Cannon } from "../Cannon.js";
import { RandomColor } from "../ChangeColor.js";
import { ChangeColorOnCollision } from "../ChangeColorOnCollision.js";
import { IncreaseShaderSpeedOverTime } from "../ChangeCustomShaderProperty.js";
import { DepthOfFieldController } from "../DepthOfFieldController.js";
import { DisableEnvironmentLight } from "../DisableEnvironment.js";
import { EmitParticlesOnClick } from "../EmitParticlesOnClick.js";
import { EnforceParameters } from "../EnforceParameters.js";
import { HTMLButtonClick } from "../HTMLButtonEvent.js";
import { IFrameContent } from "../IFrameContent.js";
import { ImageTrackingDownloadUI } from "../ImageTrackingDownloadUI.js";
import { LookAtCamera } from "../LookAtCamera.js";
import { Networking_ClickToChangeColor } from "../Networking.js";
import { Networking_StringArray } from "../Networking.js";
import { Networking_Object } from "../Networking.js";
import { PerformanceSettings } from "../PerformanceSettings.js";
import { PhysicsCollision } from "../PhysicsCollision.js";
import { PhysicsTrigger } from "../PhysicsCollision.js";
import { PlayAnimationOnCollision } from "../PlayAnimationOnCollision.js";
import { PlayAnimationOnTrigger } from "../PlayAnimationOnTrigger.js";
import { PlayAudioOnCollision } from "../PlayAudioOnCollision.js";
import { PrefabSceneSwitcherSample } from "../SceneSwitcher.js";
import { SceneSwitcherSample } from "../SceneSwitcher.js";
import { ShowBalloonMessage } from "../ShowBalloonMessage.js";
import { StartPosition } from "../StartPosition.js";
import { AutoReset } from "../StartPosition.js";
import { SyncedRoomUI } from "../SyncedRoomUI.js";
import { TimedSpawn } from "../TimedSpawn.js";
import { Variatns } from "../Variants.js";
import { VideoBackground } from "../VideoBackground.js";
import { ResetPositionOnInterval } from "../VisibilitySamples.js";
import { ToggleVisibility } from "../VisibilitySamples.js";

// Register types
TypeStore.add("Cannon", Cannon);
TypeStore.add("RandomColor", RandomColor);
TypeStore.add("ChangeColorOnCollision", ChangeColorOnCollision);
TypeStore.add("IncreaseShaderSpeedOverTime", IncreaseShaderSpeedOverTime);
TypeStore.add("DepthOfFieldController", DepthOfFieldController);
TypeStore.add("DisableEnvironmentLight", DisableEnvironmentLight);
TypeStore.add("EmitParticlesOnClick", EmitParticlesOnClick);
TypeStore.add("EnforceParameters", EnforceParameters);
TypeStore.add("HTMLButtonClick", HTMLButtonClick);
TypeStore.add("IFrameContent", IFrameContent);
TypeStore.add("ImageTrackingDownloadUI", ImageTrackingDownloadUI);
TypeStore.add("LookAtCamera", LookAtCamera);
TypeStore.add("Networking_ClickToChangeColor", Networking_ClickToChangeColor);
TypeStore.add("Networking_StringArray", Networking_StringArray);
TypeStore.add("Networking_Object", Networking_Object);
TypeStore.add("PerformanceSettings", PerformanceSettings);
TypeStore.add("PhysicsCollision", PhysicsCollision);
TypeStore.add("PhysicsTrigger", PhysicsTrigger);
TypeStore.add("PlayAnimationOnCollision", PlayAnimationOnCollision);
TypeStore.add("PlayAnimationOnTrigger", PlayAnimationOnTrigger);
TypeStore.add("PlayAudioOnCollision", PlayAudioOnCollision);
TypeStore.add("PrefabSceneSwitcherSample", PrefabSceneSwitcherSample);
TypeStore.add("SceneSwitcherSample", SceneSwitcherSample);
TypeStore.add("ShowBalloonMessage", ShowBalloonMessage);
TypeStore.add("StartPosition", StartPosition);
TypeStore.add("AutoReset", AutoReset);
TypeStore.add("SyncedRoomUI", SyncedRoomUI);
TypeStore.add("TimedSpawn", TimedSpawn);
TypeStore.add("Variatns", Variatns);
TypeStore.add("VideoBackground", VideoBackground);
TypeStore.add("ResetPositionOnInterval", ResetPositionOnInterval);
TypeStore.add("ToggleVisibility", ToggleVisibility);
