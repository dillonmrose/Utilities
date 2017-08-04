#pragma once
#include <vector>
#include <sstream>
#include <memory>
#include "LearningModel.h"

using namespace std;

class LearningModelEnsemble : public LearningModel
{
public:

    vector <shared_ptr <LearningModel> > models;
    vector <double> weights;

    explicit LearningModelEnsemble();
    virtual ~LearningModelEnsemble();
    virtual void Initialize(wistream& textFile) override;
    virtual void Initialize(vector<char>& binaryFile, size_t& pos) override;
    virtual double Predict(const vector <double>& features) const override;

    virtual void Output(wostream& textFile) const override;
    virtual void Output(vector<char>& binaryFile) const override;
    virtual void Output(wstring& code) const override;
};

class LearnEnsemble : public LearnEngine
{

public:

	LearningModelEnsemble* ensemble;
	vector <LearnEngine *> engines;

    LearnEnsemble(const vector <LearnEngine *>& engines);
	LearnEnsemble(int num, LearnEngine* engine);
    virtual void Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data) override;
};