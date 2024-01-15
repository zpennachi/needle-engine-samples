import { TypeStore } from "@needle-tools/engine"

// Import types
import { CombinedVideo } from "../CombinedVideo.js";
import { HlsVideo } from "../HlsVideo.js";

// Register types
TypeStore.add("CombinedVideo", CombinedVideo);
TypeStore.add("HlsVideo", HlsVideo);
