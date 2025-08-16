using BodyParts;
using BodyParts.Systems;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

namespace Entities {
	public class Entity : MonoBehaviour {
		public int                 AttributeMax = 12;
		public CirculatorySystem   CirculatorySystem   => gameObject.GetComponent<CirculatorySystem>();
		public SkeletalSystem      SkeletalSystem      => gameObject.GetComponent<SkeletalSystem>();
		public NervousSystem       NervousSystem       => gameObject.GetComponent<NervousSystem>();
		public MuscularSystem      MuscularSystem      => gameObject.GetComponent<MuscularSystem>();
		public IntegumentarySystem IntegumentarySystem => gameObject.GetComponent<IntegumentarySystem>();

		public    Rigidbody         RigidBody => gameObject.GetComponent<Rigidbody>();
		protected Legs              Legs      => gameObject.GetComponent<Legs>();
		public    Collider          Collider  => gameObject.GetComponent<Collider>();
		public    UnityEvent<float> Staggered = new();
		public    bool              WeakToSilver;
		protected float             staggerTime;
		public float StaggerTime{
			get => staggerTime;
			set {
				staggerTime = value;
				if (staggerTime > 0) { Staggered.Invoke(staggerTime); }
			}
		}

		[HideInInspector] public UnityEvent eDie = new UnityEvent();
		protected virtual        void       Start()              { }
		protected virtual        void       Update()             { InvoluntaryActions(); }
		protected virtual        void       VoluntaryActions()   { }
		protected virtual        void       InvoluntaryActions() { }
		protected virtual        void       Die()                { }
	}
}