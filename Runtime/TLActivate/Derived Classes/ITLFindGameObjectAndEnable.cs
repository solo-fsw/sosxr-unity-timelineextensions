using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ITLFindGameObjectAndEnable : MonoBehaviour, ITLActivate
    {
        [SerializeField] private string m_tagToFind = "Participant_Canvas_Handle_Parent";
        [SerializeField] private GameObject m_gameObject;


        public bool IsValid { get; private set; }


        [ContextMenu("Activate")]
        public void TLActivate()
        {
            FindGameObject();

            if (m_gameObject == null)
            {
                return;
            }

            var enabler = m_gameObject.GetComponent<MassEnabler>();

            if (enabler == null)
            {
                Debug.LogError("No MassEnabler found on " + m_gameObject.name + ".");

                return;
            }

            enabler.EnableAll();
        }


        public void OnValidate()
        {
            IsValid = true;
        }


        private void FindGameObject()
        {
            if (m_gameObject != null)
            {
                return;
            }

            if (GameObject.FindGameObjectWithTag(m_tagToFind) == null)
            {
                Debug.LogWarning("No gameObject found with tag " + m_tagToFind + "in scene.");

                return;
            }

            m_gameObject = GameObject.FindGameObjectWithTag(m_tagToFind);
        }
    }
}