using System;
using System.Linq;
using System.Reflection;

namespace Laba_1
{
    public static class EnemyFactory
    {
        public static IEnemy CreateEnemy(string typeName, params object[] args)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == typeName);
            if (type == null) throw new ArgumentException($"Класс {typeName} не найден");
            return (IEnemy)Activator.CreateInstance(type, args);
        }
    }
}