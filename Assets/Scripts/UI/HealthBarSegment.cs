using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class HealthBarSegment : MonoBehaviour {
		[Required] public Image         Foreground;
		[Required] public Image         Background;
		[Required] public MMF_Player    MoveUpFeedback;
		[Required] public MMF_Player    MoveDownFeedback;
		[Required] public MMF_Player    OnDecreaseStartFeedback;
		[Required] public MMF_Player    OnDecreaseStopFeedback;
		public            MMProgressBar ProgressBar => GetComponent<MMProgressBar>();
		public            float         targetFullness;
	}
}