using UnityEngine;

/// This script is for data setting inside the inspector.
///   buffs will read this data and override the original if they find this script.
/// Still this script is available in the whole game,
///   and the modification of these value will be valid for items created after.
public class BuffDataset : MonoBehaviour
{
    public float massupMassAdd;
    
    public float speedupMassMult;
    public float speedupThrustMult;
    
    public float invulnerableTime;
    
    public float defenceAdd;
}