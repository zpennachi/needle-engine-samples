import { Animator, AnimatorControllerParameterType, Parameter, Behaviour, IPointerClickHandler, Mathf, serializable, Text } from "@needle-tools/engine";
import { SyncedAnimator } from "../SyncedAnimator";

export class SyncedAnimatorControls_RandomValue extends Behaviour implements IPointerClickHandler {

    @serializable(Animator)
    animator?: Animator;

    @serializable(SyncedAnimator)
    syncAnimator?: SyncedAnimator;

    @serializable()
    animParam: string = "";

    @serializable()
    useSyncAnimAPI: boolean = false;

    @serializable(Text)
    valueLabel?: Text;

    parameter!: Parameter;

    awake(): void {
        const params = this.animator?.runtimeAnimatorController?.model?.parameters;
        if(!params) return;
        this.parameter = params.find(p => p.name === this.animParam)!;
    }

    onPointerClick(_) {
        if(!this.syncAnimator) return;
        if(!this.parameter) return;

        if(this.syncAnimator.drivenByAnimator) {
            this.syncAnimator.requestOwnership();
            if(!this.syncAnimator.ownershipModel.hasOwnership)
                return;
        }

        if(this.parameter.type == AnimatorControllerParameterType.Float || this.parameter.type == AnimatorControllerParameterType.Int) {
            const value =  Mathf.random() * 1.5;
            if(this.useSyncAnimAPI)
                this.syncAnimator.setFloat(this.animParam, value);
            else
                this.parameter.value = value;
        }
        else if(this.parameter.type == AnimatorControllerParameterType.Bool || this.parameter.type == AnimatorControllerParameterType.Trigger) {
            const value = !(this.parameter.value as boolean); //invert current value
            if(this.useSyncAnimAPI) // triggers are booleans
                this.syncAnimator.setBool(this.animParam, value);
            else
                this.parameter.value = value;
        }
    }

    update() {
        if(!this.parameter) return;
        if(!this.valueLabel) return;

        var text = "";
        if(this.parameter.type == AnimatorControllerParameterType.Float || this.parameter.type == AnimatorControllerParameterType.Int) {
            text = (this.parameter.value as number).toFixed(3);
        }
        else {
            text = `${(this.parameter.value as boolean)}`;
        }

        this.valueLabel.text = text;
    }
}

export class SyncedAnimatorControls_PlayAnim extends Behaviour implements IPointerClickHandler {

    @serializable(Animator)
    animator?: Animator;

    @serializable(SyncedAnimator)
    syncAnimator?: SyncedAnimator;

    @serializable()
    animName: string = "";

    @serializable()
    useSyncAnimAPI: boolean = false;

    onPointerClick(_) {
        if(!this.animator) return;
        if(!this.syncAnimator) return;

        if(this.syncAnimator.drivenByAnimator) {
            this.syncAnimator.requestOwnership();
            if(!this.syncAnimator.ownershipModel.hasOwnership)
                return;
        }

        if(this.useSyncAnimAPI)
            this.syncAnimator.play(this.animName);
        else
            this.animator.play(this.animName);
    }
}