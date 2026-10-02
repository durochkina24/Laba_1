using System.IO;
using System.Text.Json;

namespace Laba_1
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        public void AddEnemy(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
        {
            return enemies.Find(e => e.Name == name);
        }

        public CEnemyTemplate GetEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count) return enemies[id];
            return null;
        }

        public void DeleteEnemyByName(string name)
        {
            enemies.RemoveAll(e => e.Name == name);
        }

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count) enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            List<string> names = new List<string>();
            foreach (var enemy in enemies) names.Add(enemy.Name);
            return names;
        }

        public void SaveToJson(string path)
        {
            string jsonString = JsonSerializer.Serialize(enemies);
            File.WriteAllText(path, jsonString);
        }

        public void LoadFromJson(string path)
        {
            string jsonFromFile = File.ReadAllText(path);
            JsonDocument doc = JsonDocument.Parse(jsonFromFile);
            enemies.Clear();

            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string name = element.GetProperty("Name").GetString();
                string iconName = element.GetProperty("IconName").GetString();
                int baseLife = element.GetProperty("BaseLife").GetInt32();
                double lifeModifier = element.GetProperty("LifeModifier").GetDouble();
                int baseGold = element.GetProperty("BaseGold").GetInt32();
                double goldModifier = element.GetProperty("GoldModifier").GetDouble();
                double spawnChance = element.GetProperty("SpawnChance").GetDouble();

                enemies.Add(new CEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance));
            }
        }
    }
}
