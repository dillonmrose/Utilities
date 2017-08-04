#include "pch.h"
using namespace std;
namespace Utils
{
    void Normalize(vector <double> &v)
    {
        double s = 0;
        for (int i = 0; i < (int)v.size(); i++)
        {
            s += fabs(v[i]);
        }
        if (s > 1E-9)
        {
            for (int i = 0; i < (int)v.size(); i++)
            {
                v[i] /= s;
            }
        }
    }

    int RandInt()
    {
        return (((long long)rand() << 30ll) ^ ((long long)rand() << 15) ^ (long long)rand()) & 2147483647;
    }

    double RandDouble()
    {
        return RandInt() / 2147483647.0;
    }

    void MakePairs(const vector<vector<pair<vector<double>, double>>>& data, vector<vector<double>>& features, vector<pair<pair<int, int>, double>>& pairs)
    {
        features.clear();
        pairs.clear();
        size_t row = 0;
        for (size_t i = 0; i < data.size(); ++i)
        {
            for (size_t j = 0; j < data[i].size(); ++j)
            {
                features.push_back(data[i][j].first);
            }
            for (size_t j = 0; j < data[i].size(); ++j)
            {
                for (size_t k = 0; k < data[i].size(); ++k)
                {
                    if (data[i][j].second > data[i][k].second + 1E-5)
                    {
                        pairs.push_back(make_pair(make_pair(row + j, row + k), data[i][j].second - data[i][k].second));
                    }
                }
            }
            row += data[i].size();
        }
    }

    void MakeData(const vector<vector<double>>& features, const vector<double>& labels, vector<vector<pair<vector<double>, double>>>& data)
    {
        data.push_back(vector<pair<vector<double>, double>>());
        for (size_t i = 0; i < features.size(); ++i)
        {
            data.back().push_back(make_pair(features[i], labels[i]));
        }
    }

    void MakePoints(const vector<vector<pair<vector<double>, double>>>& data, vector<vector<double>>& features, vector<double>& labels)
    {
        features.clear();
        labels.clear();
        for (size_t i = 0; i < data.size(); ++i)
        {
            for (size_t j = 0; j < data[i].size(); ++j)
            {
                features.push_back(data[i][j].first);
                labels.push_back(data[i][j].second);
            }
        }
    }

    double Activate(double x, double threshold)
    {
        double l, r, k;
        k = 2 / max(min(threshold, 1 - threshold), 0.05);
        l = -threshold*k;
        l = 1 / (1 + exp(-l));
        r = (1 - threshold)*k;
        r = 1 / (1 + exp(-r));
        x = (x - threshold)*k;
        x = 1 / (1 + exp(-x));
        return (x - l) / (r - l);
    }

    double Activate(double x)
    {
        return Activate(x, 0.5);
    }

    double Sigma(double x, double threshold, double k)
    {
        if (k > 50)
        {
            if (x > threshold) return 1.0;
            else return 0.0;
        }
        else return 1 / (1 + exp(-(x - threshold)*k));
    }
    
    wstring Round(double x, int d)
    {
        wchar_t str[20], format[20];
        swprintf_s(format, L"%%.%dlf", d);
        swprintf_s(str, format, x);
        return str;
    }

    double DecodeFromUint16(uint16_t x)
    {
        double v = x & 1023;
        x >>= 10;
        int e = x & 31;
        x >>= 5;
        if (e == 0) e++;
        else v += 1024;
        e -= 25;
        if (e < 0) v /= 1 << -e;
        else v *= 1 << e;
        if (x & 1) v = -v;
        return v;
    }
    
    uint16_t EncodeToUint16(double x)
    {
        int s = 0;
        if (x < 0)
        {
            x = -x;
            s = 1;
        }
        int e = 0;
        for (; x > 1 && e <= 15; x /= 2)
        {
            e++;
        }
        for (; x < 1 && e >= -14; x *= 2)
        {
            e--;
        }
        int m = 0;
        if (e < -14) x /= 2;
        else x -= 1;
        for (int i = 0; i < 10; i++)
        {
            x *= 2;
            m *= 2;
            if (x >= 1)
            {
                x -= 1;
                ++m;
            }
        }
        e += 15;
        if (e < 0) e = 0;
        if (e > 31) e = 31;
        return (s << 15) | (e << 10) | m;
    }
    
    unsigned char ReadUint8(char*& buf)
    {
        unsigned char value = *(reinterpret_cast <unsigned char *>(buf));
        ++buf;
        return value;
    }

    uint16_t ReadUint16(char*& buf)
    {
        uint16_t value = *(reinterpret_cast <uint16_t *>(buf));
        buf += 2;
        return value;
    }
	
	float ReadFloat(char*& buf)
	{
		float value = *(reinterpret_cast <float *>(buf));
		buf += sizeof(float);
		return value;
	}

    void WriteUint8(char *& buf, unsigned char value)
    {
        *(reinterpret_cast<unsigned char *>(buf)) = value;
        ++buf;
    }
    
    void WriteUint16(char *& buf, uint16_t value)
    {
        *(reinterpret_cast<uint16_t *>(buf)) = value;
        buf += 2;
    }

	void WriteFloat(char*& buf, float value)
	{
		*(reinterpret_cast <float *>(buf)) = value;
		buf += sizeof(float);
	}
}