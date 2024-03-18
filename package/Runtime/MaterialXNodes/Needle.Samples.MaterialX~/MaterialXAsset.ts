import { Behaviour, Context, GameObject, INeedleGLTFExtensionPlugin, ImageReference, NeedleEngine, Renderer, SourceIdentifier, addCustomExtensionPlugin, serializable } from "@needle-tools/engine";
import { Loader, LoadingManager, Material, Texture } from "three";

import { MaterialXLoader } from 'three/examples/jsm/loaders/MaterialXLoader.js';
import { nodeFrame } from "three/examples/jsm/renderers/webgl-legacy/nodes/WebGLNodes";

import { type GLTF, type GLTFLoaderPlugin, GLTFParser, GLTFLoader } from "three/examples/jsm/loaders/GLTFLoader.js";
import { ContextEvent } from "@needle-tools/engine";

// Documentation → https://docs.needle.tools/scripting

export class MaterialXAsset extends Behaviour {

    // A .mtlx file is basically an xml file that references other files, like textures.
    // We're referencing it as "FileReference" so that it will be copied out on export.
    //@type FileReference
    @serializable(URL)
    materialXAsset?: string;

    // As a workaround for the MaterialX loader not supporting proper control over texture loading yet,
    // we need to manually assign all textures that are referenced in the .mtlx file.
    @serializable(ImageReference)
    images: Array<ImageReference> = [];
    
    start() {
        if (!this.materialXAsset) return;
        this.loadMaterialX(this.materialXAsset as any as string);
    }

    update() {
        nodeFrame.update();
    }

    async loadMaterialX(path: string) {
        const renderer = GameObject.getComponent(this.gameObject, Renderer);
        if (!renderer) return;

        const manager = new LoadingManager();

        // This is a WORKAROUND until we can properly reference compressed/packed textures (requires three.js update + MaterialX loader fixes).
        // In the meantime, they're exported using ImageReference, where they are simply put into "assets/"
        // without a subpath, so we need to assume
        // 1) all texture names are unique
        // 2) all textures needed by the .mtlx file are manually referenced in the array above
        manager.resolveURL = ( url: string ) => {
            // If the URL already starts with "assets/" we keep it as is.
            if (url.startsWith('assets/')) return url;

            // Split out the path and only use the filename for reference for now, we assume unique names here.
            const parts = url.split('/');
            const filename = parts.pop();
            console.log('resolveURL', url, filename)
            return "assets/" + filename;
        };
        
        // Load compressed/uncompressed textures from the same glTF file this component is from.
        console.log(this.sourceId);
        if (this.sourceId !== undefined) {
            const customLoader = new GLTFTextureLoader(this.sourceId, manager);
            manager.addHandler(/\.(jpg|png|exr|ktx2|webp)$/i, customLoader);
            console.log("added custom handler", customLoader, this.sourceId)
        }
        
        const mtlxLoader = new MaterialXLoader(manager);
        const material = await mtlxLoader
					.setPath( '' )
					.loadAsync( path )
					.then( ( { materials } ) => {
                        // a mtlx file can contain multiple materials, we just take the first one here and log all of them
                        console.log('Loaded .mtlx materials', materials);
                        const firstMaterial = Object.values( materials )[0] as Material;
                        return firstMaterial;
                    });

        console.log('Loaded .mtlx material', material, path);
        if (material)
            renderer.sharedMaterial = material;
    }
}

class GLTFTextureLoader extends Loader {

    private readonly sourceId: SourceIdentifier;

    constructor(sourceId: SourceIdentifier, manager?: LoadingManager) {
        super(manager);
        this.sourceId = sourceId;
    }

    load(url: string): Texture {
        console.log("custom loading:", url);
        const texture = new Texture();
        texture.name = url;
        if (!url) return texture;

        // get the current context and glTF parser
        const parser = MaterialXTextureLoaderGLTFCache.instances.get(this.sourceId);
        if (!parser) return texture;
    
        parser.json.textures.forEach((gltfTexture, index) => {
            // url is something like "assets/texture.jpg"
            // texture.name is something like "texture"
            // we need to find the texture that matches the url
            let fileNameOnly = url.split('/').pop();
            if (fileNameOnly) fileNameOnly = fileNameOnly.split('.')[0];
            
            if (url === gltfTexture.uri || (fileNameOnly && fileNameOnly === gltfTexture.name)) {
                // load the texture from the glTF file
                parser.loadTexture(index).then((resultTexture) => {
                    texture.image = resultTexture.image;
                    texture.needsUpdate = true;
                });
            }
        });

        return texture;
    }
}

class MaterialXTextureLoaderPlugin implements INeedleGLTFExtensionPlugin {
    get name(): string {
        return "NEEDLE_materialx_texture_loader";
    }

    onImport(loader: GLTFLoader, sourceId: SourceIdentifier) {
        console.log("yooo")
        loader.register(p => new MaterialXTextureLoaderGLTFCache(p, sourceId));
    }
}

class MaterialXTextureLoaderGLTFCache implements GLTFLoaderPlugin {
    get name(): string {
        return "NEEDLE_materialx_texture_loader"; 
    }

    readonly parser: GLTFParser;
    readonly sourceId: SourceIdentifier;
    static readonly instances: Map<SourceIdentifier, GLTFParser> = new Map();
    
    constructor(parser: GLTFParser, sourceId: SourceIdentifier) {
        console.log("constructing plugin")
        this.parser = parser;
        this.sourceId = sourceId;
        MaterialXTextureLoaderGLTFCache.instances.set(sourceId, parser);
    }

    // Workaround until three/types is updated to include name and constructor,
    // otherwise TypeScript complains that this is not a GLTFLoaderPlugin.
    beforeRoot() { return null; }
}

console.error("adding plugin")
addCustomExtensionPlugin(new MaterialXTextureLoaderPlugin());

NeedleEngine.registerCallback(ContextEvent.ContextRegistered, (evt) => {
    console.warn("ContextRegistered", evt);
});

NeedleEngine.registerCallback(ContextEvent.ContextCreationStart, (evt) => {
    console.warn("ContextCreationStart", evt);
});

NeedleEngine.registerCallback(ContextEvent.ContextCreated, (evt) => {
    console.warn("ContextCreated", evt);
});