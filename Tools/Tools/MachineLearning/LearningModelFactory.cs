#include "pch.h"

using namespace std;

unordered_map <wstring, LearningModelType> LearningModelFactory::modelNameToType = 
{
    {L"LinearModel", LearningModelType::LinearModelType },
    {L"DecisionTree", LearningModelType::DecisionTreeType },
    {L"LearningModelEnsemble", LearningModelType::LearningModelEnsembleType }
};

shared_ptr<LearningModel> LearningModelFactory::CreateLearningModel(LearningModelType modelType)
{
    switch (modelType)
    {
    case LinearModelType:
        return make_shared <LinearModel>();
    case DecisionTreeType:
        return make_shared <DecisionTree>();
    case LearningModelEnsembleType:
        return make_shared <LearningModelEnsemble>();
    default:
        return make_shared <LearningModel>();
    }
}
    
shared_ptr<LearningModel> LearningModelFactory::CreateLearningModel(const wstring& modelName)
{
    return CreateLearningModel(modelNameToType[modelName]);
}

shared_ptr<LearningModel> LearningModelFactory::CreateLearningModelFromTextFile(wistream& textFile)
{
    wstring name;
    textFile >> name;
    shared_ptr<LearningModel> model = CreateLearningModel(name);
    model->Initialize(textFile);
    return model;
}

shared_ptr<LearningModel> LearningModelFactory::CreateLearningModelFromBinaryFile(vector<char>& binaryFile)
{
    size_t pos = 0;
    return CreateLearningModelFromBinaryFile(binaryFile, pos);
}

shared_ptr<LearningModel> LearningModelFactory::CreateLearningModelFromBinaryFile(vector <char>& binaryFile, size_t& pos)
{
    LearningModelType type = static_cast<LearningModelType>(binaryFile[pos]);
    ++pos;
    shared_ptr<LearningModel> model = CreateLearningModel(type);
    model->Initialize(binaryFile, pos);
    return model;
}
