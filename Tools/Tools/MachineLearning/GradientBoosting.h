#pragma once

#include <iostream>
#include <vector>
#include <algorithm>
#include <sstream>
#include "LearningModelEnsemble.h"

using namespace std;

class GradientBoosting : public LearnEnsemble
{

public:

    GradientBoosting(const vector <LearnEngine *>& engines);
    GradientBoosting(int num, LearnEngine* engine);
    virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
    virtual vector<double> CalcGradient(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const = 0;
    void Learn(const vector <vector <pair <vector <double>, double> > > &learnData, const vector <vector <pair <vector <double>, double> > > & validateData);
    void Learn(const vector <vector <double> > &features, const vector <pair <pair <int, int>, double> > &pairs, const vector <vector <double> > &featuresControl, const vector <pair <pair <int, int>, double> > &pairsControl);
    virtual double CalcScore(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const;
    double CalcScore(const vector<vector<pair<vector<double>, double>>>& data, const vector<vector<double>>& values) const;
    void Learn1(const vector <vector <double> > &features, const vector <double> &values);
};

class GradientBoostingRanker : public GradientBoosting
{
    double CalcScore(const vector<double>& values) const;
    
public:
    
	GradientBoostingRanker(const vector <LearnEngine *>& engines);
    GradientBoostingRanker(int num, LearnEngine* engine);
    virtual vector<double> CalcGradient(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const override;
    virtual double CalcScore(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const override;
};

class GradientBoostingClassifier : public GradientBoosting
{
public:
    
	GradientBoostingClassifier(const vector <LearnEngine *>& engines);
    GradientBoostingClassifier(int num, LearnEngine* engine);
    virtual vector<double> CalcGradient(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const override;
    virtual double CalcScore(const vector<pair<vector<double>, double>>& data, const vector<double>& values) const override;
};
