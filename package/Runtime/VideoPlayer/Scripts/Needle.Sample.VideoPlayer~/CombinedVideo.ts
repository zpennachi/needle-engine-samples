import { Behaviour, FrameEvent, GameObject, Renderer, VideoPlayer, serializable } from "@needle-tools/engine";
import { NoColorSpace } from "three";

export class CombinedVideo extends Behaviour {

    @serializable(VideoPlayer)
    player?: VideoPlayer;

    @serializable(Renderer)
    renderer?: Renderer;

    @serializable()
    textureName: string = "_VideoTex";

    start(): void {

        const vidTexture = this.player?.videoTexture;
        if(vidTexture == null || this.renderer == null)
            return;
        
        console.log(this.renderer.sharedMaterial);
        this.renderer.sharedMaterial[this.textureName] = vidTexture;
    }
}