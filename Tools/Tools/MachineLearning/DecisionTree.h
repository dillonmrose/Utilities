#pragma once
#include <iostream>
#include <vector>
#include <algorithm>
#include "LearningModel.h"

using namespace std;

class DecisionTree : public LearningModel
{

private:

    double TraverseWithAccumulateFallback(int nodeId, const vector<double>& features) const;
    double TraverseWithZeroFallback(int nodeId, const vector<double>& features) const;
    void InitializeDfs(size_t& pos, const vector <pair <int, double> > & rawData);
    void Initialize(const vector <pair <int, double> > & rawData);
    
    void CodeDfs(int nodeId, wstring& code) const;
    void OutputDfs(int nodeId, vector <pair <int, double> > & data) const;
    void Output(vector <pair <int, double> > & data) const;

public:

    struct Node
    {
        int feature;
        double value;
        int left, right;
        Node();
    };
	vector <Node> nodes;

    explicit DecisionTree();
    virtual ~DecisionTree() {}
    virtual void Initialize(wistream& textFile) override;
    virtual void Initialize(vector<char>& binaryFile, size_t& pos) override;
    virtual double Predict(const std::vector <double>& features) const override;

    virtual void Output(wostream & file) const override;
    virtual void Output(vector<char>& binaryFile) const override;
    virtual void Output(wstring& code) const override;
};

class DecisionTreeLearn : public LearnEngine
{
    int maxDepth;
    int minLeafSize;
    double costComplexityThreshold;
    void Build(int nodeId, vector <int> trainSubset, int depth, const vector <vector <double> > &features, const vector <double> &values);
    double GetImpurity(const vector <double> &values);
    double Evaluate(int nodeId, const vector <double> &values);
	DecisionTree* decisionTree;

public:

	DecisionTreeLearn(int maxDepth, int minLeafSize);
    virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};
