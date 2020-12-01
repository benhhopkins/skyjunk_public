

public class CollectHealthEffect : HitTargetEffect {

  public int heal = 1;

  public override void Hit(HitTrigger trigger, HitTarget target) {
    target.HP += heal;
  }
}