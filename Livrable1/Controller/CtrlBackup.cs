using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Livrable1.View;

namespace Livrable1.Controller
{
    public class CtrlBackup
    {
        private List<CtrlBackup> saves = new();
        private const string FILE_PATH_SAVE = "save.json";
        public CtrlBackup()
        {
            LoadData();
        }

        public void AddBackup()
        {
            ViewAddBackup.AddBackup();
        }

        public void ExecuteBackup()
        {
            ViewExecuteBackup.ExecuteBackup();
        }

        public void RecoverBackup()
        {
            ViewRecoverBackup.RecoverBackup();
        }

        public void ChoiceLanguage()
        {
            ViewChoiceLanguage.ChoiceLanguage();
        }

        public void ShowLogs()
        {
            ViewChoiceLanguage.ChoiceLanguage();
        }

        public void LoadData()
        {
            try
            {
                if (File.Exists(FILE_PATH_SAVE))
                {
                    string json = File.ReadAllText(FILE_PATH_SAVE);
                    saves = JsonSerializer.Deserialize<List<CtrlBackup>>(json) ?? new List<CtrlBackup>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data : {ex.Message}");
            }
        }
    }
}

