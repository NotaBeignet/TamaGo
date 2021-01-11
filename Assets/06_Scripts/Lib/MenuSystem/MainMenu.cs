using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnOpen(MENUTYPE a_previousMenu)
    {
      /*  if(a_previousMenu == MENUTYPE.NOTHING)
        {
            SoundManager.Instance.StartRandomSoundPack(RANDOM_SOUND_TYPE.MENU_MUSIC, MIXER_GROUP_TYPE.AMBIANT, SOUND_PACK_ELEMENT_TYPE.LOOP1);
        }*/
        base.OnOpen(a_previousMenu);
    }
}
