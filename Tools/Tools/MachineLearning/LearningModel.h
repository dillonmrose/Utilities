#pragma once
#include <iostream>
#include <vector>
#include <algorithm>

using namespace std;
class LearningModel
{

public:

    explicit LearningModel();
    virtual ~LearningModel();
    virtual void Initialize(wistream& textFile);
    virtual void Initialize(vector<char>& binaryFile, size_t& pos);
	void Initialize(vector<char>& binaryFile);
    virtual double Predict(const vector <double>& features) const;
    
    virtual void Output(wostream &textFile) const;
    virtual void Output(vector<char>& binaryFile) const;
    virtual void Output(wstring& code) const;
};

class LearnEngine
{
public:

	virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) = 0;
};

class Metric
{
public:

    static double NDCG(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data, int k);
};