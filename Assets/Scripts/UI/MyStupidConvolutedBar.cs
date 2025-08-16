using System;
using System.Collections.Generic;
using BodyParts.Systems;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace UI {
	public class MyStupidConvolutedBar : MonoBehaviour {
		// TODO: Text box where the blood volume is displayed. 1 HP = 100 mL.
		// TODO: I'd like to make it so the damage is colored with the type and the amount reduced is colored in in a different shade, but that would require me to color a bar 2 colors at once...
		// TODO: Also actually show max values...
		[SerializeField, ShowInInspector]                          private byte                   barsPerSegment = 4; // Number of bars each segment will hold.
		[SerializeField, ShowInInspector]                          private byte                   valuePerBar    = 3;
		[SerializeField, ShowInInspector, Required, AssetSelector] private GameObject             barPrefab;
		[SerializeField, ShowInInspector]                          private List<HealthBarSegment> bars;
		[ShowInInspector, ReadOnly]                                private List<GameObject>       barsAffected;
		private                                                            float                  totalTime = 2;
		public                                                             List<HealthBarSegment> affectedBars;

		[Required, SceneObjectsOnly, ShowInInspector, HideInPrefabAssets]
		public SystemWithStat System{
			get => _system;
			set {
				if (_system != null) {
					_system.eLevelChanged.RemoveListener(updateMaxValue);
					_system.eValueDamaged.RemoveListener(ValueDamaged);
				}

				_system = value;
				if (value == null) return;
				_system.eLevelChanged.AddListener(updateMaxValue);
				_system.eValueDamaged.AddListener(ValueDamaged);
				updateMaxValue();
				updateCurrentValue();
			}
		}
		[SerializeField, HideInInspector] private SystemWithStat _system;

		[Button] private void updateMaxValue() {
			ushort temp = 0;
			for (byte i = 0; i < bars.Count; i++) {
				temp += valuePerBar;
				bars[i].gameObject.SetActive(temp <= System.MaxValue);
			}
		}

		[Button] public void SetUp() {
			updateMaxValue();
			updateCurrentValue();
			//Look through each bar, get their foreground and background, and set their color to the color of the system.
			foreach (HealthBarSegment bar in bars) {
				bar.Foreground.color = System.ValueColor;
				bar.Background.color = System.CapColor;
			}
		}

		/// <summary> This is only for when health is set directly. During playtime, you should use ValueDamaged() </summary>
		[Button] private void updateCurrentValue() {
			//Loop through each bar and get the MMBar component. Distribute System.Value across the ProgressBars.
			ushort temp = 0;
			for (byte i = 0; i < System.Level; i++) {
				MMProgressBar bar = bars[i].GetComponent<MMProgressBar>();
				temp += valuePerBar;
				if (temp <= System.Value) { bar.UpdateBar01(1); }
				else { bar.UpdateBar01((float) (System.Value - (temp - valuePerBar)) / valuePerBar); }
			}
		}

		public void Start() { System = System; }

		public void ValueDamaged(DamageType damageType, int amount) {
			affectedBars = new List<HealthBarSegment>();
			ushort   temp                  = 0;
			Gradient decreasingBarGradient = generateDamageColorGradient();
			int      oldAmount             = amount + System.Value;

			int startingIndex = (int) Math.Floor((double) (System.Value / 3));
			int finalIndex;
			if (oldAmount % valuePerBar == 0) finalIndex = oldAmount / 3 - 1;
			else finalIndex                              = (int) Math.Floor((double) (oldAmount / 3));
			for (int j = startingIndex; j <= finalIndex; j++) {
				affectedBars.Add(bars[j]);
				MMProgressBar bar = bars[j].GetComponent<MMProgressBar>();
				//Set how full the bar segment should be at the end of the animation.
				bars[j].targetFullness = Math.Max((float) (System.Value - (((j + 1) * 3) - valuePerBar)) / valuePerBar, 0);

				//Move the affected bar pieces upwards
				bars[j].MoveUpFeedback.GetFeedbackOfType<MMF_Position>().AnimatePositionDuration = 0.5f;
				bars[j].MoveUpFeedback.PlayFeedbacks();

				//Turn the colors from the health bar color to the damage color.
				// TODO fixme
				//bars[j].OnDecreaseStartFeedback.GetFeedbackOfType<MMF_Image>().ColorOverTime = decreasingBarGradient;
				//bars[j].OnDecreaseStartFeedback.GetFeedbackOfType<MMF_Image>().Duration      = (float) (0.5);
				//bars[j].OnDecreaseStartFeedback.PlayFeedbacks();
				
				bar.LerpForegroundBarDurationDecreasing =  bars[j].Foreground.fillAmount - bars[j].targetFullness;
				//bar.LerpDecreasingDelayedBarDuration    =  bar.LerpForegroundBarDurationDecreasing / 2;
				//bar.LerpDecreasingDelayedBarDuration = 0;
				// set the delay so that the bars move up and change color at the same time.
			}


			// Loop through the affected bars backwards, Start UpdateBar01 on the last of the affected bars, then make it so when it's done, it calls the next one, and so on. Otherwise the bars will all start at the same time.
			for (int i = affectedBars.Count - 1; i >= 0; i--) {
				MMProgressBar bar = affectedBars[i].GetComponent<MMProgressBar>();
				
				if (i == affectedBars.Count - 1) {
					bar.UpdateBar01(affectedBars[i].targetFullness);
				}
				else {
					int j = i;
					affectedBars[j + 1].OnDecreaseStopFeedback.Events.OnComplete.AddListener(() => affectedBars[j].GetComponent<MMProgressBar>().UpdateBar01(affectedBars[j].targetFullness));
					//remove the animation curve from all but the first
					affectedBars[j].ProgressBar.LerpForegroundBarCurveDecreasing = AnimationCurve.Linear(0f, 0f, 1f, 1f);
				}
				// if j = 0, putBarsBack() in the OnDecreaseStopFeedback
				if (i == 0) {
					affectedBars[i].OnDecreaseStopFeedback.Events.OnComplete.AddListener(putBarsBack);
					Debug.Log("added final event");
				}
			}

			void putBarsBack() {
				Debug.Log("putting bars back");
				for (int i = affectedBars.Count - 1; i >= 0; i--) { affectedBars[i].OnDecreaseStartFeedback.PlayFeedbacksInReverse(); }
			}

			// TODO: Stage 3 where all the emptied ones go to the down position and all the semi-full ones go back to centre.

			Gradient generateDamageColorGradient() {
				Gradient           testGradient = new();
				GradientColorKey[] colorKeys    = new GradientColorKey[2];
				colorKeys[0].color = System.ValueColor;
				colorKeys[0].time  = 0.0f;
				colorKeys[1].color = damageType.Color;
				colorKeys[1].time  = 1.0f;
				GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
				alphaKeys[0].alpha = 1.0f;
				alphaKeys[0].time  = 0.0f;
				alphaKeys[1].alpha = 1.0f;
				alphaKeys[1].time  = 1.0f;
				testGradient.SetKeys(colorKeys, alphaKeys);
				return testGradient;
			}
		}
	}
}