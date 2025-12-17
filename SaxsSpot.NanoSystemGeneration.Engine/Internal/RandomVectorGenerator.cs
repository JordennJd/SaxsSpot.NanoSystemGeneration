using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones.Enums;

namespace SaxsSpot.NanoSystemGeneration.Engine.Internal;

public static class RandomVectorGenerator
{
    public static List<Vector<float>> GenerateRandomVectors(int count, GenerationZone zone, double innerBound, double outerBound)
{
    var random = new Random();
    var list = new List<Vector<float>>();

    if (innerBound < 0 || outerBound <= 0 || innerBound >= outerBound)
    {
        throw new ArgumentException("innerBound must be non-negative and less than outerBound");
    }

    for (int i = 0; i < count; i++)
    {
        Vector<float> vector;

        if (zone.GenerationZoneForm == GenerationZoneForm.Cube)
        {
            // Для куба: равномерное распределение между внутренним и внешним кубом
            float scaleFactor = (float)Math.Pow(random.NextSingle() * 
                (Math.Pow(outerBound, 3) - Math.Pow(innerBound, 3)) + 
                Math.Pow(innerBound, 3), 1.0/3.0) / (float)(zone.GlobalSize / 2);

            vector = Vector<float>.Build.DenseOfArray(new float[]
            {
                (random.NextSingle() * 2 - 1) * scaleFactor * (float)zone.GlobalSize / 2,
                (random.NextSingle() * 2 - 1) * scaleFactor * (float)zone.GlobalSize / 2,
                (random.NextSingle() * 2 - 1) * scaleFactor * (float)zone.GlobalSize / 2
            });
        }
        else // Сфера
        {
            // Генерация случайного направления (единичный вектор)
            Vector<float> direction;
            do
            {
                direction = Vector<float>.Build.DenseOfArray(new float[]
                {
                    (random.NextSingle() * 2 - 1),
                    (random.NextSingle() * 2 - 1),
                    (random.NextSingle() * 2 - 1)
                });
            } while (direction.L2Norm() > 1);

            // Нормализуем вектор направления
            direction = direction.Normalize(2);

            // Генерация случайного радиуса между innerBound и outerBound
            // Равномерное распределение в объеме сферы
            float randomRadius = (float)Math.Pow(
                random.NextSingle() * (Math.Pow(outerBound, 3) - Math.Pow(innerBound, 3)) + 
                Math.Pow(innerBound, 3), 
                1.0/3.0);

            // Создаем вектор с нужным радиусом и направлением
            vector = direction * randomRadius;
        }

        list.Add(vector);
    }

    return list;
}
    public static List<Vector<float>> GenerateRandomVectors(int count, GenerationZone zone)
    {
        var random = new Random();
        var list = new List<Vector<float>>();

        for (int i = 0; i < count; i++)
        {
            Vector<float> vector;

            if (zone.GenerationZoneForm == GenerationZoneForm.Cube)
            {
                vector = Vector<float>.Build.DenseOfArray(new float[]
                {
                    (random.NextSingle() * 2 - 1) * (float)zone.GlobalSize / 2,
                    (random.NextSingle() * 2 - 1) * (float)zone.GlobalSize / 2,
                    (random.NextSingle() * 2 - 1) * (float)zone.GlobalSize / 2
                });
            }
            else
            {
                Vector<float> point;
                do
                {
                    point = Vector<float>.Build.DenseOfArray(new float[]
                    {
                        (random.NextSingle() * 2 - 1),
                        (random.NextSingle() * 2 - 1),
                        (random.NextSingle() * 2 - 1)
                    });
                } while (point.L2Norm() > 1);

                vector = point * (float)zone.GlobalSize;
            }

            list.Add(vector);
        }

        return list;
    }
}