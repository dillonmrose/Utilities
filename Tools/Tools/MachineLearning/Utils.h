#pragma once
#include <vector>

using namespace std;
namespace Utils
{
	void Normalize(vector <double> &v);
	int RandInt();
	double RandDouble();
	void MakePoints(const vector <vector <pair <vector <double>, double> > > &data, vector <vector <double> > &features, vector <double> &labels);
	void MakePairs(const vector <vector <pair <vector <double>, double> > > &data, vector <vector <double> > &features, vector <pair <pair <int, int>, double> > &pairs);
    void MakeData(const vector <vector <double> > &features, const vector <double> &labels, vector <vector <pair <vector <double>, double> > > &data);
    double Activate(double x, double threshold);
    double Activate(double x);
    double Sigma(double x, double threshold, double k);
    wstring Round(double x, int d);
    double DecodeFromUint16(uint16_t x);
    uint16_t EncodeToUint16(double x);
	float ReadFloat(char *& buf);
	void WriteFloat(char *& buf, float value);
    unsigned char ReadUint8(char*& buf);
    uint16_t ReadUint16(char*& buf);
    void WriteUint8(char*& buf, unsigned char value);
    void WriteUint16(char*& buf, uint16_t value);
}