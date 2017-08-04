using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.MachineLearning
{
    class LinearModel : LearningModel
    {
        List<double> weights;
        double threshold;
   
        public LinearModel()
        {
        }
        double Normalize(double score)
        {
            if (threshold == 0) return score;
            score = Activate(score, threshold);
            score = Math.Max(score, 0.0);
            score = Math.Min(score, 1.0);
            return score;
        }

        void Initialize(string file)
        {
            int size;
            textFile >> size;
            weights.resize(size);
            for (auto & weight : weights)
            {
                textFile >> weight;
            }
            textFile >> threshold;
        }

        void Output(string file)
        {
            textFile << L"LinearModel\n";
            textFile << weights.size() << L"\n";
	        for (auto & weight : weights)
            {
		        textFile << Round(weight, 6) << L"\n";
	        }
            textFile << Round(threshold, 6) << L"\n";
        }


        double Predict(List<double> features)
        {
            double score = 0;
            for (int i = 0; i < features.Count && i < weights.Count; ++i)
                {
                score += features[i] * weights[i];
            }
            return Normalize(score);
        }
}
/*
double LinearModelLearn::GetAverageScore(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) const
{
    double max_rank = 0;
    for (auto & q : data)
    {
        for (auto & e : q)
        {
            max_rank = max(max_rank, model->Predict(e.first));
        }
    }
    return max_rank / 2;
}


void LinearRankerLearnPointwise::Learn(LearningModel* model, const vector<vector<pair<vector<double>, double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <double> labels;
	MakePoints(data, features, labels);
    int n = features[0].size();
    linearModel->weights.resize(n);
	
    vector <double> bestW(n);
    double bestEr = 1E20;
    
	double margin = 1E-4;
	int numOfPhases = 20;
	int numOfIterations = 500;
	int numOfGradientSteps = 50;
	int randomSubsetSize = 200;
	
	for (int phase = 0; phase < numOfPhases; phase++)
    {
        for (int i = 0; i < n; i++)
        {
            linearModel->weights[i] = RandDouble();
        }
        Utils::Normalize(linearModel->weights);
        for (int iteration = 0; iteration < numOfIterations; iteration++)
        {
            for (int gradientStep = 0; gradientStep < numOfGradientSteps; gradientStep++)
            {
                vector <double> gradient(n, 0);
                vector <int> subset(randomSubsetSize);
                vector <double> m(randomSubsetSize);
				for (int i = 0; i < randomSubsetSize; i++)
				{
					int queryId = RandInt() % features.size();
					double s = linearModel->Predict(features[queryId]);
					s *= 10;
					subset[i] = queryId;
					m[i] = min(exp(-s * labels[queryId]), 10.0);
				}
                for (int i = 0; i < randomSubsetSize; i++)
				{
				    int queryId = subset[i];
                	for (int j = 0; j < n; j++)
                    {
                        gradient[j] -= features[queryId][j] * labels[queryId] * m[i];
                    }
                }
				Utils::Normalize(gradient);
				double gradientCoef = 1.0 / (gradientStep + 2.0);
				for (int j = 0; j < n; j++)
                {
                    linearModel->weights[j] -= gradient[j] * gradientCoef;
					linearModel->weights[j] = max(linearModel->weights[j], 0.0);
                }
				Utils::Normalize(linearModel->weights);
			}
			double er = 0;
            for (int i = 0; i < (int)features.size(); i++)
            {
                double s = linearModel->Predict(features[i]);
                s *= 10;
				er += min(exp(-s * labels[i]), 10.0);
            }
            if (er < bestEr)
            {
                bestEr = er;
                bestW = linearModel->weights;
            }
			//wcout << bestEr << L"\n";
        }
    }
    linearModel->weights = bestW;
    Utils::Normalize(linearModel->weights);
    linearModel->threshold = GetAverageScore(model, data) * 0.7;
}


double LinearRankerLearnPairwise::CalcScore(LinearModel * linearModel, const vector<vector<pair<vector<double>, double>>>& data)
{
	double score = 0;
	for (auto & query : data)
	{
		vector <pair <double, double> > scores;
		for (auto & entity : query)
		{
			scores.push_back(make_pair(linearModel->Predict(entity.first), -entity.second));
		}
		if (!scores.empty())
		{
			sort(scores.begin(), scores.end());
			reverse(scores.begin(), scores.end());
			double sum = 0;
			double sumDiscount = 0;
			for (int i = 0; i < 1 && i < (int)scores.size(); i++)
			{
				double discount = log(2.0) / log(2.0 + i);
				sum += -scores[i].second * discount;
				sumDiscount += discount;
			}
			score += sum / sumDiscount;
		}
	}
	return score;
}

void LinearRankerLearnPairwise::Learn(LearningModel* model, const vector<vector<pair<vector<double>, double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <pair <pair <int, int>, double> > pairs;
	MakePairs(data, features, pairs);
	wcout << L"Number of pairs = " << pairs.size() << L"\n";
	int n = features[0].size();
	linearModel->weights.resize(n);
	linearModel->threshold = 0;

	double bestScore = 0;
	vector <double> bestWeights = linearModel->weights;

	double margin = 1E-4;
	int numOfIterations = 2000;
	int numOfGradientSteps = 200;
	int randomSubsetSize = 200;

	for (int iteration = 0; iteration < numOfIterations; iteration++)
	{
		for (auto & weight : linearModel->weights)
		{
			weight = RandDouble();
		}
		Utils::Normalize(linearModel->weights);
		for (int gradientStep = 0; gradientStep < numOfGradientSteps; gradientStep++)
		{
			vector <double> gradient(n, 0);
			vector <int> subsetIds(randomSubsetSize);
			vector <double> lossCoef(randomSubsetSize);
			for (int i = 0; i < randomSubsetSize; i++)
			{
				int pairId = RandInt() % pairs.size();
				int id1 = pairs[pairId].first.first;
				int id2 = pairs[pairId].first.second;
				double s = linearModel->Predict(features[id2]) - linearModel->Predict(features[id1]);
				s *= 20;
				subsetIds[i] = pairId;
				lossCoef[i] = min(exp(s), 20.0);
			}
			for (int i = 0; i < randomSubsetSize; i++)
			{
				int pairId = subsetIds[i];
				int id1 = pairs[pairId].first.first;
				int id2 = pairs[pairId].first.second;
				for (int j = 0; j < n; j++)
				{
					gradient[j] += pairs[pairId].second * pairs[pairId].second * (features[id2][j] - features[id1][j]) * lossCoef[i];
				}
			}
			Utils::Normalize(gradient);
			double gradientCoef = 1.0 / (gradientStep + 10.0);
			for (int j = 0; j < n; j++)
			{
				linearModel->weights[j] -= gradient[j] * gradientCoef;
				linearModel->weights[j] = max(linearModel->weights[j], 0.0);
			}
			Utils::Normalize(linearModel->weights);
		}
		double score = CalcScore(linearModel, data);
		if (score > bestScore)
		{
			bestScore = score;
			bestWeights = linearModel->weights;
		}
	}
	linearModel->weights = bestWeights;
	Utils::Normalize(linearModel->weights);
	linearModel->threshold = GetAverageScore(model, data) * 0.7;
}

void LinearClassifierLearn::Learn(LearningModel* model, const vector<vector<pair<vector<double>,double>>>& data)
{
    LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <double> labels;
	MakePoints(data, features, labels);
    int n = features[0].size();
    linearModel->weights.resize(n);
	linearModel->threshold = 0;

    vector <double> bestW(n);
    double bestEr = 1E20;
    
	double margin = 1E-4;
	int numOfPhases = 50;
	int numOfIterations = 500;
	int numOfGradientSteps = 50;
	int randomSubsetSize = 200;
	
	for (int phase = 0; phase < numOfPhases; phase++)
    {
        for (int i = 0; i < n; i++)
        {
            linearModel->weights[i] = RandDouble();
        }
        Utils::Normalize(linearModel->weights);
        for (int iteration = 0; iteration < numOfIterations; iteration++)
        {
            for (int gradientStep = 0; gradientStep < numOfGradientSteps; gradientStep++)
            {
                vector <double> gradient(n, 0);
                vector <int> subset(randomSubsetSize);
                vector <double> m(randomSubsetSize);
				for (int i = 0; i < randomSubsetSize; i++)
				{
					int queryId = RandInt() % features.size();
					double s = linearModel->Predict(features[queryId]);
					s *= 10;
					double sign = 0;
					if (labels[queryId] > 0) sign = 1;
					if (labels[queryId] < 0) sign = -1;
					subset[i] = queryId;
					m[i] = min(exp(-s * sign), 10.0);
				}
                for (int i = 0; i < randomSubsetSize; i++)
				{
				    int queryId = subset[i];
                	for (int j = 0; j < n; j++)
                    {
                        gradient[j] -= features[queryId][j] * labels[queryId] * m[i];
                    }
                }
				Utils::Normalize(gradient);
				double gradientCoef = 1.0 / (gradientStep + 2.0);
				for (int j = 0; j < n; j++)
                {
                    linearModel->weights[j] -= gradient[j] * gradientCoef;
					linearModel->weights[j] = max(linearModel->weights[j], 0.0);
                }
				Utils::Normalize(linearModel->weights);
			}
			double er = 0;
            for (int i = 0; i < (int)features.size(); i++)
            {
                double s = linearModel->Predict(features[i]);
                double sign = 0;
                if (labels[i] > 0) sign = 1;
                if (labels[i] < 0) sign = -1;
                double weight = fabs(labels[i]);
                er += (s * sign <= margin) * weight;
            }
            if (er < bestEr)
            {
                bestEr = er;
                bestW = linearModel->weights;
            }
			//wcout << bestEr << L"\n";
        }
    }
    linearModel->weights = bestW;
    Utils::Normalize(linearModel->weights);
    linearModel->threshold = 0;
}

void LinearSymmetricConfidenceRegressionLearn::Learn(LearningModel* model, const vector<vector<pair<vector<double>,double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <double> labels;
	MakePoints(data, features, labels);
	int n = features[0].size();
	linearModel->weights.resize(n);
	vector <double> bestW(n);
	double bestEr = -1E20;
	for (int it = 0; it < 200; it++)
	{
		for (int i = 0; i < n; i++)
		{
			linearModel->weights[i] = rand();
		}
		Utils::Normalize(linearModel->weights);
		for (int step = 1; step <= 100; step++)
		{
			vector <double> m(features.size());
			double er = 0;
			for (int i = 0; i < (int)features.size(); i++)
			{
				double s = linearModel->Predict(features[i]) * (1 - 1E-8) + 1E-8;
				er += log((1 + s) / 2);
				m[i] = 1 + s;
			}
			if (er > bestEr)
			{
				bestEr = er;
				bestW = linearModel->weights;
			}
			vector <double> g(linearModel->weights.size(), 0);
			for (int l = 0; l < n; l++)
			{
				for (int i = 0; i < (int)features.size(); i++)
				{
					g[l] -= features[i][l] / m[i];
				}
			}
			Utils::Normalize(g);
			for (int l = 0; l < n; l++)
			{
				linearModel->weights[l] = max(linearModel->weights[l] - g[l], 0.0);
			}
			Utils::Normalize(linearModel->weights);
		}
	}
    linearModel->threshold = 0;
}

void LinearConfidenceRegressionLearn::Learn(LearningModel* model, const vector<vector<pair<vector<double>,double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <double> labels;
	MakePoints(data, features, labels);
	int n = features[0].size();
    linearModel->weights.resize(n);
    vector <double> bestW(n);
    double bestEr = -1E20;
    for (int it = 0; it < 100; it++)
    {
        for (int i = 0; i < n; i++)
        {
            linearModel->weights[i] = rand();
        }
        Utils::Normalize(linearModel->weights);
        for (int step = 1; step <= 100; step++)
        {
            double er = 0;
            for (int i = 0; i < (int)features.size(); i++)
            {
                double s = linearModel->Predict(features[i]) * (1 - 2E-8) + 1E-8;
                if (labels[i] < 0) s = 1 - s;
                er += log(s);
            }
            //wcout << L"E: " << er << L"\n";
            if (er > bestEr)
            {
                bestEr = er;
                bestW = linearModel->weights;
            }
            for (int step2 = 1; step2 <= 50; step2++)
            {
                vector <double> g(linearModel->weights.size(), 0);
                vector <int> subset;
                vector <double> m;
                for (int j = 0; j < 100; j++)
                {
                    int i = RandInt() % features.size();
                    subset.push_back(i);
                    double s = linearModel->Predict(features[i]) * (1 - 2E-8) + 1E-8;
                    if (labels[i] < 0) s = 1 - s;
                    m.push_back(s);
                }
                for (int l = 0; l < n; l++)
                {
                    for (int j = 0; j < (int)subset.size(); j++)
                    {
                        int i = subset[j];
                        g[l] -= labels[i] * features[i][l] / m[j];
                    }
                }
                Utils::Normalize(g);
                for (int l = 0; l < n; l++)
                {
                    linearModel->weights[l] = linearModel->weights[l] - g[l] / sqrt(step2);
                    linearModel->weights[l] = max(linearModel->weights[l], 0.0);
                }
                Utils::Normalize(linearModel->weights);
            }
        }
    }
    linearModel->weights = bestW;
    linearModel->threshold = 0;
    wprintf(L"%lf\n", bestEr);
    for (int i = 0; i < n; i++)
    {
        wprintf(L"%lf\n", linearModel->weights[i]);
    }
}

void LinearRegressionLearnByGauss::Learn(LearningModel* model, const vector<vector<pair<vector<double>,double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <double> values;
	MakePoints(data, features, values);
    int n = features[0].size();
    vector <vector <double> > a(n, vector <double>(n + 1, 0.0));
    for (int j = 0; j < n; j++)
    {
        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < (int)features.size(); i++)
            {
                a[j][k] += features[i][k] * features[i][j];
            }
        }
        for (int i = 0; i < (int)features.size(); i++)
        {
            a[j][n] += features[i][j] * values[i];
        }
    }
    double mx = 0;
    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            mx = max(mx, fabs(a[i][j]));
        }
    }
    for (int i = 0; i < n; i++)
    {
        a[i][i] += mx / n / n;
    }
    vector <int> p(n);
    for (int i = 0; i < n; i++)
    {
        int bj = 0;
        for (int j = 0; j < n; j++)
            if (fabs(a[i][j]) > fabs(a[i][bj])) bj = j;
        if (fabs(a[i][bj]) < 1E-9) wprintf(L"Error\nSystem is inconsistent\n");
        else
        {
            p[i] = bj;
            for (int k = 0; k < n; k++)
            {
                if (k != i)
                {
                    double t = a[k][bj] / a[i][bj];
                    for (int j = 0; j <= n; j++)
                    {
                        a[k][j] -= t*a[i][j];
                    }
                }
            }
        }
    }
    linearModel->weights.resize(n);
    linearModel->threshold = 0;
    for (int i = 0; i < n; i++)
    {
        linearModel->weights[p[i]] = a[i][n] / a[i][p[i]];
    }
    double er = 0;
    for (int i = 0; i < (int)features.size(); i++)
    {
        double s = linearModel->Predict(features[i]);
        er += (s - values[i])*(s - values[i]);
    }
    //wcout << L"Error: " << er / features.size() << L"\n";
}

void LinearRegressionLearnByGradientDescent::Learn(LearningModel* model, const vector<vector<pair<vector<double>,double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	vector <vector <double> > features;
	vector <double> values;
	MakePoints(data, features, values);
    int n = features[0].size();
    linearModel->weights.resize(n);
    vector <double> bestW(n);
    double bestEr = 1E20;
    int neg = 0, pos = 0, zer = 0;
    for (int i = 0; i < n; i++)
    {
        linearModel->weights[i] = RandDouble();
    }
    Utils::Normalize(linearModel->weights);
    for (int step = 1; step <= 200; step++)
    {
        double er = 0;
        for (int i = 0; i < (int)features.size(); i++)
        {
            double s = linearModel->Predict(features[i]);
            er += (s - values[i])*(s - values[i]);
        }
        if (er < bestEr)
        {
            bestEr = er;
            bestW = linearModel->weights;
        }
        for (int step2 = 1; step2 <= 500; step2++)
        {
            vector <double> g(linearModel->weights.size(), 0);
            vector <int> step;
            vector <double> m;
            for (int j = 0; j < 100; j++)
            {
                int i = RandInt() % features.size();
                step.push_back(i);
                double s = linearModel->Predict(features[i]);
                m.push_back(s - values[i]);
            }
            for (int l = 0; l < n; l++)
            {
                for (int j = 0; j < (int)step.size(); j++)
                {
                    int i = step[j];
                    g[l] += features[i][l] * m[j];
                }
            }
            Utils::Normalize(g);
            for (int l = 0; l < n; l++)
            {
                linearModel->weights[l] = linearModel->weights[l] * 0.95 - g[l] * 0.2;
            }
            Utils::Normalize(linearModel->weights);
        }
    }
    linearModel->weights = bestW;
    linearModel->threshold = 0;
    //Utils::Normalize(weights);
}

double LinearRankerLearnListwise::CalcScore(LinearModel * linearModel, const vector<pair<vector<double>, double>>& query)
{
	vector <pair <double, double> > scores;
	for (auto & entity : query)
	{
		scores.push_back(make_pair(linearModel->Predict(entity.first), -entity.second));
	}
	sort(scores.begin(), scores.end());
	reverse(scores.begin(), scores.end());
	double sum = 0;
	double sumDiscount = 0;
	for (int i = 0; i < 1 && i < (int)scores.size(); i++)
	{
		double discount = log(2.0) / log(2.0 + i);
		sum += -scores[i].second * discount;
		sumDiscount += discount;
	}
	return sum / sumDiscount;
}

double LinearRankerLearnListwise::CalcScore(LinearModel * linearModel, const vector<vector<pair<vector<double>, double>>>& data)
{
	double score = 0;
	for (auto & query : data)
	{
		score += CalcScore(linearModel, query);
	}
	return score;
}

void LinearRankerLearnListwise::Learn(LearningModel * model, const vector<vector<pair<vector<double>, double>>>& data)
{
	LinearModel* linearModel = (LinearModel *)model;
	int n = data[0][0].first.size();
	linearModel->weights.resize(n);
	linearModel->threshold = 0;

	double bestScore = 0;
	vector <double> bestWeights = linearModel->weights;

	int numOfPhases = 50;
	int numOfIterations = 500;
	int numOfGradientSteps = 50;
	int randomSubsetSize = 50;
	for (int phase = 0; phase < numOfPhases; phase++)
	{
		for (auto & weight : linearModel->weights)
		{
			weight = RandDouble();
		}
		Utils::Normalize(linearModel->weights);
		for (int iteration = 0; iteration < numOfIterations; iteration++)
		{

			for (int gradientStep = 0; gradientStep < numOfGradientSteps; gradientStep++)
			{
				//wcout << CalcScore(linearModel, data) << L"\n";
				vector <double> gradient(n, 0);
				for (int i = 0; i < randomSubsetSize; i++)
				{
					int queryId = RandInt() % data.size();
					auto & query = data[queryId];
					vector <pair <double, int> > ranking, idealRanking;
					for (int k = 0; k < (int)query.size(); k++)
					{
						ranking.push_back(make_pair(linearModel->Predict(query[k].first), k));
						idealRanking.push_back(make_pair(query[k].second, k));
					}
					sort(ranking.rbegin(), ranking.rend());
					sort(idealRanking.rbegin(), idealRanking.rend());
					int id1 = idealRanking[0].second;
					int id2 = ranking[0].second;
					double diff = query[id1].second - query[id2].second;
					for (int j = 0; j < n; j++)
					{
						gradient[j] += diff * (query[id2].first[j] - query[id1].first[j]);
					}
				}
				Utils::Normalize(gradient);
				double gradientCoef = 1.0 / (gradientStep + 2.0);
				for (int j = 0; j < n; j++)
				{
					linearModel->weights[j] -= gradient[j] * gradientCoef;
					linearModel->weights[j] = max(linearModel->weights[j], 0.0);
				}
				Utils::Normalize(linearModel->weights);
			}
			double score = CalcScore(linearModel, data);
			if (score > bestScore)
			{
				bestScore = score;
				bestWeights = linearModel->weights;
			}
			//wcout << bestScore << L"\n";
		}
	}
	linearModel->weights = bestWeights;
	Utils::Normalize(linearModel->weights);
	linearModel->threshold = GetAverageScore(model, data) * 0.7;
}
*/
}