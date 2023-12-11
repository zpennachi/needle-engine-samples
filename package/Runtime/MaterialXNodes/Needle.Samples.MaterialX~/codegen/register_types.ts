import { TypeStore } from "@needle-tools/engine"

// Import types
import { MaterialXAsset } from "../MaterialXAsset.js";
import { ShadeWithMaterialX } from "../MaterialXNodeGraph.js";

// Register types
TypeStore.add("MaterialXAsset", MaterialXAsset);
TypeStore.add("ShadeWithMaterialX", ShadeWithMaterialX);
