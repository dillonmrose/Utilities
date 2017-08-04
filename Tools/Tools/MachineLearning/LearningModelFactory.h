#pragma once
#include <vector>
#include <string>
#include <sstream>
#include <unordered_map>
#include <memory>
#include "LearningModel.h"

enum LearningModelType
{
    LinearModelType = 0,
    DecisionTreeType = 1,
    LearningModelEnsembleType = 2
};

class LearningModelFactory
{
    static unordered_map <wstring, LearningModelType> modelNameToType;
    
public:
    
    static shared_ptr<LearningModel> CreateLearningModel(LearningModelType modelType);
    static shared_ptr<LearningModel> CreateLearningModel(const wstring& modelName);
    static shared_ptr<LearningModel> CreateLearningModelFromTextFile(wistream& textFile);
    static shared_ptr<LearningModel> CreateLearningModelFromBinaryFile(vector <char>& binaryFile);
    static shared_ptr<LearningModel> CreateLearningModelFromBinaryFile(vector <char>& binaryFile, size_t& pos);
};
