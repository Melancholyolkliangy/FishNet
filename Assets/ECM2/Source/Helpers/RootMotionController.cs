using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EasyCharacterMovement
{
    /// <summary>
    ///
    /// RootMotionController.
    /// 
    /// Helper component to get Animator root motion velocity vector (animRootMotionVelocity).
    /// 
    /// This must be attached to a game object with an Animator component.
    /// </summary>

    [RequireComponent(typeof(Animator))]
    public class RootMotionController : MonoBehaviour
    {
#pragma warning disable UNT0001
        // ACT只需要取消rootMotion即可
        private void OnAnimatorMove()
        {
            // cancel root motion
        }
#pragma warning restore UNT0001
#if UNITY_EDITOR
        [Button("初始化参数")]
        public void InitPara()
        {
            var animator = this.GetComponent<Animator>();
            bool isEnemy = !this.name.ToLower().Contains("role");
            
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            animator.cullingMode = ((!isEnemy) || this.name.ToLower().Contains("Boss".ToLower())) ? AnimatorCullingMode.AlwaysAnimate : AnimatorCullingMode.CullUpdateTransforms;

            EditorUtility.SetDirty(this);
            
        }
#endif
        //#region FIELDS

        //protected Animator _animator;

        //protected Vector3 _rootMotionDeltaPosition;
        //protected Quaternion _rootMotionDeltaRotation;

        //#endregion

        //#region METHOD

        ///// <summary>
        ///// Flush any accumulated deltas. This prevents accumulation while character is toggling root motion.
        ///// </summary>

        //public virtual void FlushAccumulatedDeltas()
        //{
        //    _rootMotionDeltaPosition = Vector3.zero;
        //    _rootMotionDeltaRotation = Quaternion.identity;
        //}

        ///// <summary>
        ///// Return current root motion rotation and clears accumulated delta rotation.
        ///// </summary>

        //public virtual Quaternion ConsumeRootMotionRotation()
        //{
        //    Debug.Log("ConsumeRootMotionRotation in fixed? " + Time.inFixedTimeStep);
        //    Quaternion rootMotionRotation = _rootMotionDeltaRotation;
        //    _rootMotionDeltaRotation = Quaternion.identity;

        //    return rootMotionRotation;
        //}

        ///// <summary>
        ///// Get the calculated root motion velocity.
        ///// </summary>

        //public virtual Vector3 GetRootMotionVelocity(float deltaTime = 0.0f)
        //{
        //    if (deltaTime == 0.0f)
        //        deltaTime = Time.deltaTime;

        //    Vector3 rootMotionVelocity = _rootMotionDeltaPosition / deltaTime;
        //    return rootMotionVelocity;
        //}

        ///// <summary>
        ///// Return current root motion velocity and clears accumulated delta positions.
        ///// </summary>

        //public virtual Vector3 ConsumeRootMotionVelocity(float deltaTime = 0.0f)
        //{
        //    Vector3 rootMotionVelocity = GetRootMotionVelocity(deltaTime);
        //    _rootMotionDeltaPosition = Vector3.zero;

        //    return rootMotionVelocity;
        //}

        //#endregion

        //#region MONOBEHAVIOUR

        //public virtual void Awake()
        //{
        //    _animator = GetComponent<Animator>();

        //    if (_animator == null)
        //    {
        //        Debug.LogError($"RootMotionController: There is no 'Animator' attached to the '{name}' game object.\n" +
        //                       $"Please attach a 'Animator' to the '{name}' game object");
        //    }
        //}

        //public virtual void Start()
        //{
        //    _rootMotionDeltaPosition = Vector3.zero;
        //    _rootMotionDeltaRotation = Quaternion.identity;
        //}

        //public virtual void OnAnimatorMove()
        //{
        //    // Accumulate root motion deltas between character updates 

        //    _rootMotionDeltaPosition += _animator.deltaPosition;
        //    _rootMotionDeltaRotation = _animator.deltaRotation * _rootMotionDeltaRotation;
        //}

        //#endregion
    }
}