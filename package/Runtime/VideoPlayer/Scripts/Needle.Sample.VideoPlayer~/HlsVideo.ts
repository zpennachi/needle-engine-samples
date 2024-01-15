import { AudioSource, Behaviour, serializable } from "@needle-tools/engine";
import { VideoTexture, SRGBColorSpace, MeshBasicMaterial } from "three";

// Documentation → https://docs.needle.tools/scripting

export class HlsVideo extends Behaviour {

    @serializable()
    url: string = "https://cph-msl.akamaized.net/hls/live/2000341/test/master.m3u8";
    
    private _videoElement?: HTMLVideoElement;
    private _videoTexture?: VideoTexture;

    async start() {
        // load hls.js
        const scriptElement = document.createElement("script");
        scriptElement.src = "https://cdn.jsdelivr.net/npm/hls.js@1";
        scriptElement.onload = () => this.onHlsLoaded();
        document.head.appendChild(scriptElement);

        // wait for audio being allowed
        AudioSource.registerWaitForAllowAudio(() => {
            if (this._videoElement) this._videoElement.muted = false;
        });
    }

    private onHlsLoaded() {
        // create video element
        this._videoElement = document.createElement("video");
        this._videoElement.muted = true;
        // hide it
        this._videoElement.style.userSelect = "none";
        this._videoElement.style.visibility = "hidden";
        this._videoElement.style.display = "none";
        this.context.domElement?.prepend(this._videoElement);

        // create HLS instance
        const hls = new Hls();
        hls.loadSource(this.url);
        hls.attachMedia(this._videoElement);

        // play
        this._videoElement.play();

        // video texture
        this._videoTexture = new VideoTexture(this._videoElement);
        this._videoTexture.flipY = false;
        this._videoTexture.colorSpace = SRGBColorSpace;

        // set texture
        const mat = new MeshBasicMaterial();
        this.gameObject.material = mat;
        mat.map = this._videoTexture;
        mat.needsUpdate = true;
    }
}