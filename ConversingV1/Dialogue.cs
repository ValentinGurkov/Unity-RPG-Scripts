using UnityEngine;

namespace ConversingV1
{
    [System.Serializable]
    public struct Dialogue
    {
        [SerializeField] private string name;
        [TextArea(3, 10)] [SerializeField] private string[] dialogueLines;

        public Dialogue(string n, string[] d)
        {
            name = n;
            dialogueLines = d;
        }

        public string[] DialogueLines => dialogueLines;
        public string Name => name;
    }
}