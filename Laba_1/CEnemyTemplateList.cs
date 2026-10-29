using System.Collections.Generic;

namespace Laba_1
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;
        private readonly ISaveList<List<CEnemyTemplate>> _saver; 

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
            _saver = new JsonEnemySaver(); 
        }

        public int Count => enemies.Count;
        public List<CEnemyTemplate> GetAll() => enemies;

        public void AddEnemy(CEnemyTemplate template)
        {
            enemies.Add(template);
        }

        
        public void AddEnemy(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            AddEnemy(new CNormalEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name) => enemies.Find(e => e.Name == name);
        public void DeleteEnemyByName(string name) => enemies.RemoveAll(e => e.Name == name);

        public List<string> GetListOfEnemyNames()
        {
            var names = new List<string>();
            foreach (var e in enemies) names.Add(e.Name);
            return names;
        }

        public void NormalizeChances()
        {
            if (enemies.Count == 0) return;
            double sum = 0;
            foreach (var e in enemies) sum += e.SpawnChance;
            if (sum <= 0) return;
            foreach (var e in enemies) e.NormalizeChance(sum);
        }

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

       
        public void SaveToJson(string path) => _saver.Save(enemies, path);

        public void LoadFromJson(string path)
        {
            enemies = _saver.Load(path);
            NormalizeChances();
        }
    }
}