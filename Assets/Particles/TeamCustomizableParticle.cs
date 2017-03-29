using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCustomizableParticle : MonoBehaviour {

    public Color TeamBlueColor = new Color(0f, 0.901960784f, 0.941176471f);
    public Color TeamPurpleColor = new Color(1f, 0.078431373f, 0.670588235f);

    public bool UseGradient = false;
    public Gradient TeamBlueGradient;
    public Gradient TeamPurpleGradient;

    public void CustomizeForTeam(Team team)
    {
        var system = GetComponent<ParticleSystem>();
        var mainSystem = system.main;

        if (UseGradient)
        {
            if (team == Team.Blue)
            {
                mainSystem.startColor = new ParticleSystem.MinMaxGradient(TeamBlueGradient);
            }
            else if (team == Team.Purple)
            {
                mainSystem.startColor = new ParticleSystem.MinMaxGradient(TeamPurpleGradient);
            }
        }
        else
        {
            if (team == Team.Blue)
            {
                mainSystem.startColor = new ParticleSystem.MinMaxGradient(TeamBlueColor,TeamBlueColor);
            }
            else if (team == Team.Purple)
            {
                mainSystem.startColor = new ParticleSystem.MinMaxGradient(TeamPurpleColor,TeamPurpleColor);
            }
        }
    }
}
