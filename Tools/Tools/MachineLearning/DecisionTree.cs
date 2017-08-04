
namespace Tools.MachineLearning
{
    class DecisionTree
    {
        class Node
        {
            int feature = -1, left = -1, right = -1;
            double value = 0;
        }

        DecisionTree()
        {
        }

        void InitializeDfs(int pos, const List<pair<int, double>>& rawData)
        {
            Node node = nodes[pos];
            node.feature = rawData[pos].first;
            node.value = rawData[pos].second;
            ++pos;
            if (node.feature != -1)
            {
                node.left = pos;
                InitializeDfs(pos, rawData);
                node.right = pos;
                InitializeDfs(pos, rawData);
            }
        }

void DecisionTree::Initialize(const vector <pair <int, double> > & rawData)
{
    nodes.resize(rawData.size());
    size_t pos = 0;
    InitializeDfs(pos, rawData);
}

void DecisionTree::OutputDfs(int nodeId, vector<pair<int, double>>& data) const
{
    const Node & node = nodes[nodeId];
    data.push_back(make_pair(node.feature, node.value));
    if (node.feature != -1)
    {
        OutputDfs(node.left, data);
        OutputDfs(node.right, data);
    }
}

void DecisionTree::CodeDfs(int nodeId, wstring & code) const
{
    const Node & node = nodes[nodeId];
    code += L"(";
    if (node.feature == -1) code += to_wstring(node.value);
    else
    {
        code += L"f[" + to_wstring(node.feature) + L"]<" + to_wstring(node.value) + L"?";
        CodeDfs(node.left, code);
        code += L":";
        CodeDfs(node.right, code);
    }
    code += L")";
}

void DecisionTree::Output(vector<pair<int, double>>& data) const
{
    OutputDfs(0, data);
}

void DecisionTree::Initialize(wistream & textFile)
{
    int size;
    textFile >> size;
    vector <pair <int, double> > rawData(size);
    for (auto & node : rawData)
    {
        textFile >> node.first >> node.second;
    }
    Initialize(rawData);
}

void DecisionTree::Initialize(vector<char>& binaryFile, size_t& pos)
{
    char *buf = &binaryFile[pos];
    uint16_t size = ReadUint16(buf);
    vector <pair <int, double> > rawData(size);
    for (auto & node : rawData)
    {
        node.first = ReadUint8(buf);
        if (node.first == 255) node.first = -1;
        node.second = DecodeFromUint16(ReadUint16(buf));
    }
    pos += buf - &binaryFile[pos];
    Initialize(rawData);
}

void DecisionTree::Output(wostream & file) const
{
    file << L"DecisionTree\n";
    vector <pair <int, double> > data;
    OutputDfs(0, data);
    file << data.size() << L"\n";
    for (auto & node : data)
    {
       	file << node.first << L"\t" << Round(node.second, 6) << L"\n";
    }
}
    
void DecisionTree::Output(vector<char>& binaryFile) const
{
    vector <pair <int, double> > rawData;
    OutputDfs(0, rawData);
    size_t size = 1 + 2 + rawData.size() * (1 + 2);
    vector <char> buffer(size);
    char *buf = &buffer[0];
    WriteUint8(buf, static_cast <unsigned char>(LearningModelType::DecisionTreeType));
    WriteUint16(buf, static_cast <uint16_t>(rawData.size()));
    for (auto & node : rawData)
    {
        WriteUint8(buf, static_cast<unsigned char>(node.first));
        WriteUint16(buf, EncodeToUint16(node.second));
    }
    binaryFile.insert(binaryFile.end(), buffer.begin(), buffer.end());
}

void DecisionTree::Output(wstring & code) const
{
    CodeDfs(0, code);
}

double DecisionTree::TraverseWithAccumulateFallback(int nodeId, const vector<double>& features) const
{
    const Node & node = nodes[nodeId];
    int size = features.size();
    double value = 0.0;
    if (node.feature == -1) value = node.value;
    else if (node.feature >= size)
    {
        value += TraverseWithAccumulateFallback(node.left, features);
        value += TraverseWithAccumulateFallback(node.right, features);
        value /= 2;
    }
    else if (features[node.feature] < node.value) value = TraverseWithAccumulateFallback(node.left, features);
    else value = TraverseWithAccumulateFallback(node.left, features);
    return value;
}

double DecisionTree::TraverseWithZeroFallback(int nodeId, const vector<double>& features) const
{
    int size = features.size();
    for (; ;)
    {
        const Node & node = nodes[nodeId];
        if (node.feature == -1) return node.value;
        if (node.feature >= size) return 0.0;
        if (features[node.feature] < node.value) nodeId = node.left;
        else nodeId = node.right;
    }
    return 0.0;
}

double DecisionTree::Predict(const vector<double>& features) const
{
    if (nodes.empty()) return 0;
    return TraverseWithZeroFallback(0, features);
}
}
DecisionTreeLearn::DecisionTreeLearn(int maxDepth, int minLeafSize) : maxDepth(maxDepth), minLeafSize(minLeafSize)
{
}

double Impurity1(const vector <double> &values)
{
    double sum = 0;
    for (int i = 0; i < (int)values.size(); i++)
    {
        sum += values[i];
    }
    sum /= values.size();
    double result = 0;
    for (int i = 0; i < (int)values.size(); i++)
    {
        result += (values[i] - sum)*(values[i] - sum);
    }
    return result;
}

double Impurity2(const vector <double> &values)
{
    vector <double> copy = values;
    sort(copy.begin(), copy.end());
    vector <double> sum(copy.size() + 1);
    sum[0] = 0;
    for (int i = 0; i < (int)copy.size(); i++)
    {
        sum[i + 1] = sum[i] + copy[i];
    }
    double result = 0;
    for (int i = 0; i < (int)copy.size(); i++)
    {
        result += copy[i] * i - sum[i];
    }
    return result / (copy.size() + 1.0);
}

double Impurity3(const vector <double> &values)
{
    vector <double> copy = values;
    sort(copy.begin(), copy.end());
    double sum = 0;
    int num;
    for (int i = 0; i < (int)copy.size(); i += num)
    {
        for (num = 0; i + num < (int)copy.size() && fabs(copy[i] - copy[i + num]) < 1E-3; num++);
        sum -= (double)num / copy.size()*log((double)num / copy.size());
    }
    return sum;
}

double DecisionTreeLearn::GetImpurity(const vector <double> &values)
{
    return Impurity1(values);
}

double DecisionTreeLearn::Evaluate(int nodeId, const vector <double> &values)
{
    DecisionTree::Node& node = decisionTree->nodes[nodeId];
    node.value = 0;
    for (auto value : values)
    {
        node.value += value;
    }
    if (values.size()>0) node.value /= values.size();
    double misclass = 0;
    for (int i = 0; i < (int)values.size(); i++)
    {
        misclass += (values[i] - node.value)*(values[i] - node.value);
    }
    return misclass;
}

void DecisionTreeLearn::Build(int nodeId, vector <int> trainSubset, int depth, const vector <vector <double> > &features, const vector <double> &values)
{
    vector <double> rank_values(trainSubset.size());
    for (size_t i = 0; i < trainSubset.size(); i++)
    {
        rank_values[i] = values[trainSubset[i]];
    }
    double misclass = Evaluate(nodeId, rank_values);
    if (depth >= maxDepth || (int)trainSubset.size() <= minLeafSize) return;
    double best_threshold;
    double best_impurity = 1E10;
    int best_feature = -1;
    int num_features = features[0].size();
    for (int iter = 0; iter < 30; iter++)
    {
        int rand_feature = rand() % num_features;
        vector <double> feature_values(trainSubset.size());
        for (int i = 0; i < (int)trainSubset.size(); i++)
        {
            feature_values[i] = features[trainSubset[i]][rand_feature];
        }
        vector <double> thresholds = feature_values;
        sort(thresholds.begin(), thresholds.end());
        vector <double> unique_thresholds;
        for (int i = 0; i < (int)thresholds.size(); i++)
        {
            if (i == 0 || thresholds[i] > unique_thresholds.back() + 1E-5) unique_thresholds.push_back(thresholds[i]);
        }
        if (unique_thresholds.size() <= 1) continue;
        int step = 1;
        int num_thresholds = 50;
        if ((int)unique_thresholds.size() > num_thresholds) step = unique_thresholds.size() / num_thresholds;
        for (int i = (int)unique_thresholds.size() / 4; i + 1 < (int)unique_thresholds.size() && i < (int)unique_thresholds.size() * 3 / 4; i += step)
        {
            if (unique_thresholds[i] + 1E-5 < unique_thresholds[i + 1])
            {
                double current_threshold = (unique_thresholds[i + 1] + unique_thresholds[i]) / 2;
                vector <double> left, right;
                for (int j = 0; j < (int)feature_values.size(); j++)
                {
                    if (feature_values[j] < current_threshold) left.push_back(rank_values[j]);
                    else right.push_back(rank_values[j]);
                }
                double impurity = GetImpurity(left) + GetImpurity(right);
                if (impurity < best_impurity)
                {
                    best_impurity = impurity;
                    best_threshold = current_threshold;
                    best_feature = rand_feature;
                }
            }
        }
    }
    if (best_feature == -1) return;
    decisionTree->nodes[nodeId].value = best_threshold;
    decisionTree->nodes[nodeId].feature = best_feature;
    decisionTree->nodes[nodeId].left = decisionTree->nodes.size();
    decisionTree->nodes.push_back(DecisionTree::Node());
    decisionTree->nodes[nodeId].right = decisionTree->nodes.size();
    decisionTree->nodes.push_back(DecisionTree::Node());
    
    vector <int> left_subset, right_subset;
    for (int i = 0; i < (int)trainSubset.size(); i++)
    {
        if (features[trainSubset[i]][best_feature] < best_threshold) left_subset.push_back(trainSubset[i]);
        else right_subset.push_back(trainSubset[i]);
    }
    Build(decisionTree->nodes[nodeId].left, left_subset, depth + 1, features, values);
    Build(decisionTree->nodes[nodeId].right, right_subset, depth + 1, features, values);
}

void DecisionTreeLearn::Learn(LearningModel* model, const vector <vector <pair <vector <double>, double> > > &data)
{
	decisionTree = (DecisionTree *)model;
    vector <vector <double> > features;
    vector <double> values;
    for (auto & q : data)
    {
        for (auto & e : q)
        {
            features.push_back(e.first);
            values.push_back(e.second);
        }
    }
    vector <int> train_set(features.size());
    for (int i = 0; i < (int)train_set.size(); i++)
    {
        train_set[i] = i;
    }
    decisionTree->nodes.clear();
    decisionTree->nodes.push_back(DecisionTree::Node());
    Build(0, train_set, 0, features, values);
}
}