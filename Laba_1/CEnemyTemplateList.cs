using System.Collections.Generic;
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

        public int Count => enemies.Count;
        public List<CEnemyTemplate> GetAll() => enemies;

        public void AddEnemy(string name, string iconName, int baseLife,
            double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(name, iconName, baseLife,
                lifeModifier, baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
            => enemies.Find(e => e.Name == name);

        public CEnemyTemplate GetEnemyByIndex(int id)
            => (id >= 0 && id < enemies.Count) ? enemies[id] : null;

        public void DeleteEnemyByName(string name)
            => enemies.RemoveAll(e => e.Name == name);

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count) enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            var names = new List<string>();
            foreach (var e in enemies) names.Add(e.Name);
            return names;
        }

        // ---------- Нормализация шансов ----------
        public void NormalizeChances()
        {
            if (enemies.Count == 0) return;
            double sum = 0;
            foreach (var e in enemies) sum += e.SpawnChance;
            if (sum <= 0) return;
            foreach (var e in enemies) e.NormalizeChance(sum);
        }

        // ---------- Выбор случайного шаблона по накопленной вероятности ----------
        public CEnemyTemplate FindByChance(double chance)
        {
            double sum = 0;
            foreach (var e in enemies)
            {
                sum += e.SpawnChance;
                if (sum >= chance) return e;
            }
            return enemies.Count > 0 ? enemies[enemies.Count - 1] : null;
        }

        // ---------- JSON ----------
        public void SaveToJson(string path)
        {
            string json = JsonSerializer.Serialize(enemies,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public void LoadFromJson(string path)
        {
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<List<CEnemyTemplate>>(json);
            if (loaded != null)
            {
                enemies = loaded;
                NormalizeChances();
            }
        }
    }
}