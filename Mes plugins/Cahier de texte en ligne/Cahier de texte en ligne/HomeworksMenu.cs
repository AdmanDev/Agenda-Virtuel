using Agenda_Virtuel;
using Agenda_Virtuel.Plugin;
using Agenda_Virtuel.Manager;
using System;

namespace Cahier_de_texte_en_ligne
{
    [HomeworksMenu]
    public static class HomeworksMenu
    {
        [HomeworksMenuItem("Supprimer définitivement")]
        public static void Delete(IHomeworkViewer hv)
        {
            HomeworkManager.DeleteHomework(hv.Homework);
            Main.hmsDeletedDefinitely.Add(hv.Homework);
            Main.instance.SetSetting("hmsDeletedDefinitely", Main.hmsDeletedDefinitely);
           Save.SaveData();
        }
    }
}