#pragma once
#include <iostream>
#include <vector>
#include <algorithm>
#include "LearningModel.h"

class LinearModel : public LearningModel
{
    double Normalize(double value) const;
    
public:

	vector <double> weights;
    double threshold;

    explicit LinearModel() {}
    virtual ~LinearModel() {}
    virtual void Initialize(wistream& textFile) override;
    virtual void Initialize(vector<char>& binaryFile, size_t& pos) override;
    virtual double Predict(const vector <double>& features) const override;

    virtual void Output(wostream &textFile) const override;
    virtual void Output(vector<char>& binaryFile) const override;
    virtual void Output(wstring& code) const override;
};

class LinearModelLearn: public LearnEngine
{
public:

	double GetAverageScore(LearningModel* model, const vector<vector<pair<vector<double>, double>>>& data) const;
};

class LinearRankerLearnPointwise : public LinearModelLearn
{
public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};

class LinearRankerLearnPairwise : public LinearModelLearn
{

	double CalcScore(LinearModel* linearModel, const vector <vector <pair <vector <double>, double> > > &data);

public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};

class LinearRankerLearnListwise : public LinearModelLearn
{

	double CalcScore(LinearModel* linearModel, const vector <vector <pair <vector <double>, double> > > &data);
	double CalcScore(LinearModel* linearModel, const vector <pair <vector <double>, double> > &query);
	double CalcIdealScore(LinearModel* linearModel, const vector <pair <vector <double>, double> > &query);

public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};

class LinearClassifierLearn : public LinearModelLearn
{
public:
	
	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};
    
class LinearSymmetricConfidenceRegressionLearn : public LinearModelLearn
{
public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};

class LinearConfidenceRegressionLearn : public LinearModelLearn
{
public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};

class LinearRegressionLearnByGauss : public LinearModelLearn
{
public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};

class LinearRegressionLearnByGradientDescent : public LinearModelLearn
{
public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};
