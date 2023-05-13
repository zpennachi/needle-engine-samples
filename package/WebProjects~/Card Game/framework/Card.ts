import { AssetReference, Behaviour, Canvas, CanvasGroup, GameObject, IPointerEventHandler, ImageReference, PointerEventData, RectTransform, Text, serializable } from "@needle-tools/engine";
import { Object3D } from "three";
import { DragHandler } from "./DragHandler";
import { type Ability } from "./Ability";

const canvasGroup: CanvasGroup = new CanvasGroup();
canvasGroup.interactable = false;
canvasGroup.blocksRaycasts = false;

export class CardModel {
    private _name?: string;
    get name() {
        if (this._name !== undefined) return this._name;
        let name = this.model?.uri?.split("/").pop();
        if (name) {
            name = name.split(".")[0];
            return this._name = name;
        }
        return this.model.uri;
    }

    get id() {
        return this.model?.uri ?? "";
    }

    @serializable(ImageReference)
    image!: ImageReference;

    @serializable(AssetReference)
    model!: AssetReference;

    idleAnimation?: string;
    abilities: Ability[] = [];

    async createTexture() {
        if (this.image)
            return this.image.createTexture();
        return null;
    }
}


export class Card extends Behaviour implements IPointerEventHandler {

    @serializable(RectTransform)
    rendering!: RectTransform;

    model?: CardModel;

    @serializable(Text)
    text?: Text;

    private _isDragging: boolean = false;
    private _originalParent: Object3D | undefined;

    get rt() {
        return this.rendering;
    }

    onDestroy(): void {
        GameObject.destroy(this.rt.gameObject)
    }

    awake(): void {
        if (!(this.rendering instanceof RectTransform) && this.rendering) {
            this.rendering = GameObject.getComponent(this.rendering, RectTransform)!;
        }
    }

    start() {
        this.name = this.model?.name ?? "Card";
    }

    onPointerDown(e: PointerEventData) {
        e.use();
        this._isDragging = true;
        this._originalParent = this.rt?.parent?.gameObject;
        const canvas = this.rt?.canvas;
        if (canvas) {
            this.rt?.markDirty();
            canvas.gameObject.add(this.rt.gameObject);
            GameObject.addComponent(this.rt.gameObject, canvasGroup);
            DragHandler.startDragging(this);
        }


    }

    onPointerUp(e: PointerEventData) {
        e.use();
        this._isDragging = false;
        DragHandler.cancel();
        GameObject.removeComponent(canvasGroup)!;
        this.context.input.setCursorNormal();
        if (this._originalParent && !this.destroyed) {
            this._originalParent.add(this.rt.gameObject);
        }
        this.rt.gameObject.position.set(0, 0, 0);
        this.rt.anchoredPosition.set(0, 0);
        this.rt.markDirty();
    }

    update(): void {
        if (!this.rt) return;
        if (!this._isDragging) return;
        const delta = this.context.input.getPointerPositionDelta(0);
        this.rt.gameObject.position.x -= delta!.x;
        this.rt.gameObject.position.y += delta!.y;
    }
}