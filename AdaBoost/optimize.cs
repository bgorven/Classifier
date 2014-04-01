namespace AdaBoost
{
    class optimize
    {
        //private Tuple<float, float, float> optimize(Tuple<float, float, float> biasAndCoefficient, IEnumerable<Tuple<TrainingSample<Sample>, float>> predictions)
        //{
        //    float cost = getLoss(biasAndCoefficient, predictions);
        //    float bias = biasAndCoefficient.Item1;
        //    float coef = biasAndCoefficient.Item2;

        //    float averagePrediction = 0;
        //    foreach (var p in predictions)
        //    {
        //        averagePrediction += p.Item2 * p.Item2;
        //    }
        //    averagePrediction /= predictions.Count();

        //    while (true)
        //    {
        //        bool biasInflection = false;
        //        bool coefInflection = false;
        //        float biasCost = 0;
        //        float coefCost = 0;
        //        foreach (var p in predictions)
        //        {
        //            float d = lossFunction.derivative(p.Item1.confidence, p.Item2 * coef + bias, p.Item1.actual);

        //            biasCost += d; //increasing the bias by x should increase the error function by x*the derivative.

        //            biasCost = Math.Max(Math.Min(biasCost, float.MaxValue), float.MinValue);

        //            if (float.IsNaN(biasCost))
        //            {
        //                throw new Exception();
        //            }
        //        }

        //        bias -= biasCost;
        //        if (float.IsNaN(bias))
        //        {
        //            throw new Exception();
        //        }

        //        float newCost = getLoss(new Tuple<float, float, float>(bias, coef, 0), predictions);
        //        if (newCost >= cost)
        //        {
        //            biasInflection = true;
        //            bias += biasCost;
        //            if (float.IsNaN(bias))
        //            {
        //                throw new Exception();
        //            }
        //        }
        //        else { cost = newCost; }

        //        foreach (var p in predictions)
        //        {
        //            float d = lossFunction.derivative(p.Item1.confidence, p.Item2 * coef + bias, p.Item1.actual);

        //            coefCost += p.Item2 * d;  //increasing the coefficient will increase or decrease the error by a
        //            //different amount depending on the value of the prediction.

        //            coefCost = Math.Max(Math.Min(coefCost, float.MaxValue), float.MinValue);

        //            if (float.IsNaN(coefCost))
        //            {
        //                throw new Exception();
        //            }
        //        }

        //        coef -= coefCost;

        //        if (float.IsNaN(coef))
        //        {
        //            throw new Exception();
        //        }

        //        newCost = getLoss(new Tuple<float, float, float>(bias, coef, 0), predictions);
        //        if (newCost >= cost)
        //        {
        //            coefInflection = true;
        //            coef += coefCost;
        //            if (float.IsNaN(coef))
        //            {
        //                throw new Exception();
        //            }
        //        }
        //        else { cost = newCost; }

        //        if ((biasInflection && coefInflection) || Math.Abs(coef) > coefLimit / averagePrediction)
        //        {
        //            if (Math.Abs(coef * averagePrediction) < 0.1)
        //            {
        //                ;
        //            }
        //            return new Tuple<float, float, float>(bias, coef, 0);
        //        }
        //    }
        //}
    }
}
