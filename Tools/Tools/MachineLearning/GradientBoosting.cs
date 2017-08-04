#include "pch.h"

using namespace std;
using namespace Utils;

GradientBoosting::GradientBoosting(const vector<LearnEngine*>& engines) : LearnEnsemble(engines)
{
}

GradientBoosting::GradientBoosting(int num, LearnEngine * engine) : LearnEnsemble(num, engine)
{
}

void GradientBoosting::Learn(LearningModel* model, const vector<vector<pair<vector<double>, double>>>& data)
{
    ensemble = (LearningModelEnsemble *)model;
    ensemble->models.resize(engines.size());
    ensemble->weights.resize(engines.size());
    int threshold = data.size() * 2 / 3;
    vector<vector<pair<vector<double>, double>>> learnData;
    learnData.insert(learnData.end(), data.begin(), data.begin() + threshold);
    vector<vector<pair<vector<double>, double>>> validateData;
    validateData.insert(validateData.end(), data.begin() + threshold, data.end());
    Learn(learnData, validateData);
    wprintf(L"NGCDLearn = %lf\tNDCGValidation = %lf\tNDCG = %lf\n", Metric::NDCG(model, learnData, 1), Metric::NDCG(model, validateData, 1), Metric::NDCG(model, data, 1));
}

double findAlpha(const vector <double> &a, const vector <double> &b)
{
    //sum( a * e^(b * alpha) ) -> min
    double best_value = 1E10;
    double best_alpha = 1;
    int lim = 10;
    for (int i = -lim; i <= lim; i++)
    {
        double alpha;
        alpha = exp(i*0.3);
        double value = 0;
        for (int j = 0; j < (int)a.size(); j++)
        {
            value += a[j] * exp(b[j] * alpha);
        }
        if (value < best_value)
        {
            best_value = value;
            best_alpha = alpha;
        }
    }
    return best_alpha;
}

double findAlpha2(const vector <double> &w, const vector <double> &a, const vector <double> &g, const vector <double> &v)
{
    //sum( w * (a + g * alpha != v) ) -> min
    double best_value = 1E10;
    double best_alpha = 1;
    int lim = 10;
    for (int i = -lim; i <= lim; i++)
    {
        double alpha;
        alpha = exp(i*0.3);
        double value = 0;
        for (int j = 0; j < (int)a.size(); j++)
        {
            value += w[j] * ((a[j] + alpha*g[j])*v[j] < 0);
        }
        if (value < best_value)
        {
            best_value = value;
            best_alpha = alpha;
        }
    }
    return best_alpha;
}

double findAlpha3(const vector <pair <pair <int, int>, double> > &p, const vector <double> &a, const vector <double> &g)
{
    //sum( w * (a + g * alpha != v) ) -> min
    double best_value = 1E10;
    double best_alpha = 1;
    int lim = 10;
    for (int i = -lim; i <= lim; i++)
    {
        double alpha;
        alpha = exp(i*0.3);
        double value = 0;
        for (int j = 0; j < (int)p.size(); j++)
        {
            double e = exp((a[p[j].first.second] + alpha*g[p[j].first.second]) - (a[p[j].first.first] + alpha*g[p[j].first.first]));
            value +=p[j].second * e;
        }
        if (value < best_value)
        {
            best_value = value;
            best_alpha = alpha;
        }
    }
    return best_alpha;
}

void GradientBoosting::Learn(const vector <vector <double> > &features, const vector <pair <pair <int, int>, double> > &pairs, const vector <vector <double> > &featuresControl, const vector <pair <pair <int, int>, double> > &pairsControl)
{
    int n = features.size();
    int m = pairs.size();
    vector <double> accumulate(n, 0.0);
    vector <double> accumulateControl(featuresControl.size(), 0.0);
    for (size_t i = 0; i < ensemble->models.size(); i++)
    {
        vector <double> gradient(n, 0.0);
        for (int j = 0; j < m; j++)
        {
            pair <int, int> p = pairs[j].first;
            double e = exp(accumulate[p.second] - accumulate[p.first]);
            e = min(e, 20.0);
            double grad = pairs[j].second * e;
            gradient[p.first] += grad;
            gradient[p.second] -= grad;
        }

        vector <vector <pair <vector <double>, double> > > data;
        MakeData(features, gradient, data);
        engines[i]->Learn(ensemble->models[i].get(), data);
        
		for (int j = 0; j < n; j++)
        {
            gradient[j] = ensemble->models[i]->Predict(features[j]);
        }

        ensemble->weights[i] = 1.0 / (i + 4); //findAlpha3(pairs, accumulate, gradient);
        for (int j = 0; j < n; j++)
        {
            accumulate[j] += ensemble->weights[i] * gradient[j];
        }
        for (int j = 0; j < (int)featuresControl.size(); j++)
        {
            accumulateControl[j] += ensemble->weights[i] * ensemble->models[i]->Predict(featuresControl[j]);
        }
        double sum = 0;
        for (auto & pair : pairs)
        {
            auto p = pair.first;
            sum += pair.second * (accumulate[p.second] > accumulate[p.first]);
        }
        wprintf(L"%lf\t", sum);
        sum = 0;
        for (auto & pair : pairsControl)
        {
            auto p = pair.first;
            sum += pair.second * (accumulateControl[p.second] > accumulateControl[p.first]);
        }
        wprintf(L"%lf\t", sum);
        wprintf(L"%lf\n", ensemble->weights[i]);
    }
}

double GradientBoosting::CalcScore(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const
{
    return 0;
}
double GradientBoosting::CalcScore(const vector <vector <pair <vector <double>, double> > >& data, const vector <vector <double> >& values) const
{
    double score = 0;
    for (size_t j = 0; j < data.size(); j++)
    {
        score += CalcScore(data[j], values[j]);
    }
    return score;
}

void GradientBoosting::Learn(const vector <vector <pair <vector <double>, double> > > &learnData, const vector <vector <pair <vector <double>, double> > > & validateData)
{
    vector <vector <double> > learnValue;
    vector <vector <double> > validateValue;
    vector <double> gradient;
    vector <vector <double> > features;
    for (const auto & v : learnData)
    {
        learnValue.push_back(vector <double>(v.size(), 0.0));
        for (const auto & f : v)
        {
            features.push_back(f.first);
            gradient.push_back(0.0);
        }
    }
    for (const auto & v : validateData)
    {
        validateValue.push_back(vector <double>(v.size(), 0.0));
    }
    for (size_t i = 0; i < ensemble->models.size(); ++i)
    {
        size_t t = 0;
        for (size_t j = 0; j < learnData.size(); j++)
        {
            auto g = CalcGradient(learnData[j], learnValue[j]);
            for (auto & x : g)
            {
                gradient[t] = x;
                ++t;
            }
        }

        vector <vector <pair <vector <double>, double> > > data;
        MakeData(features, gradient, data);
        engines[i]->Learn(ensemble->models[i].get(), data);
        
        ensemble->weights[i] = 5.0 / (i + 5); //findAlpha3(pairs, accumulate, gradient);
        
        for (size_t j = 0; j < learnData.size(); j++)
        {
            for (size_t k = 0; k < learnData[j].size(); ++k)
            {
                learnValue[j][k] += ensemble->weights[i] * ensemble->models[i]->Predict(learnData[j][k].first);
            }
        }
        
        for (size_t j = 0; j < validateData.size(); j++)
        {
            for (size_t k = 0; k < validateData[j].size(); ++k)
            {
                validateValue[j][k] += ensemble->weights[i] * ensemble->models[i]->Predict(validateData[j][k].first);
            }
        }
        
        wprintf(L"%lf\t", CalcScore(learnData, learnValue));

        wprintf(L"%lf\t", CalcScore(validateData, validateValue));

        wprintf(L"%lf\n", ensemble->weights[i]);
    }
}

void GradientBoosting::Learn1(const vector <vector <double> > &features, const vector <double> &values)
{
    int n = values.size();
    vector <double> accumulate(n, 0.0);
    for (size_t i = 0; i < ensemble->models.size(); i++)
    {
        vector <double> gradient(n);
        for (int j = 0; j < n; j++)
        {
            gradient[j] = values[j] - accumulate[j];
        }
        
        vector <vector <pair <vector <double>, double> > > data;
        MakeData(features, gradient, data);
		engines[i]->Learn(ensemble->models[i].get(), data);

        for (int j = 0; j < n; j++)
        {
            gradient[j] = ensemble->models[i]->Predict(features[j]);
        }
        double a_num = 0, a_den = 0;
        for (int j = 0; j < n; j++)
        {
            a_num += values[j] * gradient[j] - accumulate[j] * gradient[j];
            a_den += gradient[j] * gradient[j];
        }
        ensemble->weights[i] = a_num / a_den;
        for (int j = 0; j < n; j++)
        {
            accumulate[j] += ensemble->weights[i] * gradient[j];
        }
        double sum = 0;
        for (int j = 0; j < n; j++)
        {
            sum += (values[j] - accumulate[j])*(values[j] - accumulate[j]);
        }
        wprintf(L"%lf\n", sum);
    }
}

GradientBoostingClassifier::GradientBoostingClassifier(const vector<LearnEngine*>& engines) : GradientBoosting(engines)
{
}

GradientBoostingClassifier::GradientBoostingClassifier(int num, LearnEngine * engine) : GradientBoosting(num, engine)
{
}

vector<double> GradientBoostingClassifier::CalcGradient(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const
{
    vector <double> gradient(data.size(), 0.0);
    for (size_t i = 0; i < data.size(); i++)
    {
        double sign;
        if (data[i].second < 0) sign = -1;
        else sign = 1;
        double e = exp(-values[i] * sign);
        e = min(e, 20.0);
        e *= data[i].second;
        gradient[i] += e;
    }
    return gradient;
}

double GradientBoostingClassifier::CalcScore(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const
{
    double score = 0;
    for (size_t i = 0; i < data.size(); i++)
    {
        double sign;
        if (data[i].second < 0) sign = -1;
        else sign = 1;
        double e = exp(-values[i] * sign);
        e = min(e, 20.0);
        e *= data[i].second;
        score += fabs(e);
    }
    return score;
}

GradientBoostingRanker::GradientBoostingRanker(const vector<LearnEngine*>& engines) : GradientBoosting(engines)
{
}

GradientBoostingRanker::GradientBoostingRanker(int num, LearnEngine * engine) : GradientBoosting(num, engine)
{
}

vector<double> GradientBoostingRanker::CalcGradient(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const
{
    vector <double> gradient(data.size(), 0.0);
    for (size_t i = 0; i < data.size(); i++)
    {
        for (size_t j = 0; j < data.size(); j++)
        {
            if (data[i].second > data[j].second)
            {
                double e = exp(values[j] - values[i]);
                e = min(e, 20.0);
                e*=(data[i].second - data[j].second);
                gradient[i] += e;
                gradient[j] -= e;
            }
        }
    }
    return gradient;
}

double GradientBoostingRanker::CalcScore(const vector <double> & values) const
{
    double score = 0;
    double sum = 1E-7;
    for (size_t i = 0; i < values.size() && i < 1; ++i)
    {
        double discount = log(2.0) / log(i + 2);
        score += values[i] * discount;
        sum += discount;
    }
    return score / sum;
}
double GradientBoostingRanker::CalcScore(const vector <pair <vector <double>, double> > & data, const vector <double> & values) const
{
    vector <pair <double, int> > res;
    for (size_t i = 0; i < data.size(); ++i)
    {
        res.push_back({ values[i], i });
    }
    sort(res.begin(), res.end());
    reverse(res.begin(), res.end());
    vector <double> v;
    for (auto & p : res)
    {
        v.push_back(data[p.second].second);
    }
    return CalcScore(v);
}