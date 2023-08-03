﻿import { TypeStore } from "@needle-tools/engine"

// Import types
import { SyncedAnimator } from "../Animator/SyncedAnimator.js";
import { SyncedAnimator_Model } from "../Animator/SyncedAnimator_Model.js";
import { SyncedAnimatorControls_RandomValue } from "../Animator/Controls/SyncedAnimatorControls.js";
import { SyncedAnimatorControls_PlayAnim } from "../Animator/Controls/SyncedAnimatorControls.js";
import { TriggerEventListOnClick } from "../Messages/TriggerEventListOnClick.js";
import { PlayerStateUI } from "../Player/PlayerStateUI.js";
import { SampleNetworkedPlayer } from "../Player/SampleNetworkedPlayer.js";
import { RoomManager } from "../Rooms/RoomManager.js";

// Register types
TypeStore.add("SyncedAnimator", SyncedAnimator);
TypeStore.add("SyncedAnimator_Model", SyncedAnimator_Model);
TypeStore.add("SyncedAnimatorControls_RandomValue", SyncedAnimatorControls_RandomValue);
TypeStore.add("SyncedAnimatorControls_PlayAnim", SyncedAnimatorControls_PlayAnim);
TypeStore.add("TriggerEventListOnClick", TriggerEventListOnClick);
TypeStore.add("PlayerStateUI", PlayerStateUI);
TypeStore.add("SampleNetworkedPlayer", SampleNetworkedPlayer);
TypeStore.add("RoomManager", RoomManager);
