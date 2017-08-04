#include "pch.h"

using namespace std;
using namespace Utils;

LearningModelEnsemble::LearningModelEnsemble()
{
}

LearningModelEnsemble::~LearningModelEnsemble()
{
}

void LearningModelEnsemble::Initialize(wistream& textFile)
{
    int size;
    textFile >> size;
    weights.resize(size);
    for (auto & weight : weights)
    {
        textFile >> weight;
    }
    models.resize(size);
    for (auto & model : models)
    {
        model = LearningModelFactory::CreateLearningModelFromTextFile(textFile);
    }
}

void LearningModelEnsemble::Initialize(vector<char>& binaryFile, size_t& pos)
{
    char *buf = &binaryFile[pos];
    uint16_t size = ReadUint16(buf);
    weights.resize(size);
    for (auto & weight : weights)
    {
        weight = ReadFloat(buf);
    }
    pos += buf - &binaryFile[pos];
    models.resize(size);
    for (auto & model : models)
    {
        model = LearningModelFactory::CreateLearningModelFromBinaryFile(binaryFile, pos);
    }
}

void LearningModelEnsemble::Output(wostream& textFile) const
{
    textFile << L"LearningModelEnsemble\n";
    textFile << weights.size() << L"\n";
    for (auto & weight : weights)
    {
        textFile << Utils::Round(weight, 6) << L"\n";
    }
    for (auto & model : models)
    {
        model->Output(textFile);
    }
}

void LearningModelEnsemble::Output(vector<char>& binaryFile) const
{
    size_t size = 1 + 2 + weights.size() * sizeof(float);
    vector <char> buffer(size);
    char *buf = &buffer[0];
    WriteUint8(buf, static_cast <char>(LearningModelType::LearningModelEnsembleType));
    WriteUint16(buf, static_cast <uint16_t>(weights.size()));
    for (auto & weight : weights)
    {
        WriteFloat(buf, weight);
    }
    for (auto & model : models)
    {
        model->Output(buffer);
    }
    binaryFile.insert(binaryFile.end(), buffer.begin(), buffer.end());
}

void LearningModelEnsemble::Output(wstring& code) const
{
    for (size_t i = 0; i < models.size(); i++)
    {
        if (i > 0 && weights[i] > 0) code += L'+';
        code += to_wstring(weights[i]);
        code += L"*(";
        models[i]->Output(code);
        code += L")";
    }
}

double LearningModelEnsemble::Predict(const vector<double>& features) const
{
    if (models.size() != weights.size()) return 0;
    double score = 0;
    for (size_t i = 0; i < models.size(); ++i)
    {
        score += models[i]->Predict(features) * weights[i];
    }
    return score;
}

LearnEnsemble::LearnEnsemble(const vector <LearnEngine *>& engines)
{
	this->engines = engines;
}

LearnEnsemble::LearnEnsemble(int num, LearnEngine* engine)
{
	this->engines = vector <LearnEngine *>(num, engine);
}

void LearnEnsemble::Learn(LearningModel* model, const vector<vector<pair<vector<double>, double>>>& data)
{
	ensemble = (LearningModelEnsemble *)model;
	ensemble->models.resize(engines.size());
	ensemble->weights.resize(engines.size());
    for (size_t i=0; i<engines.size(); ++i)
    {
		engines[i]->Learn(ensemble->models[i].get(), data);
        ensemble->weights[i] = 1.0 / engines.size();
    }
}
