namespace dpenner1.PrimellF

open System.Text

type TruthDefinition = 
  {
    PrimesAreTruth: bool;  // If true, only primes are true, if false, non-zero is true
    EmptyIsTruth: bool;
    RequireAllTruth: bool;
  }

type PrimellConfiguration =
  {
    FreeSource: bool;
    TruthDefinition: TruthDefinition;
    InputBase: int;
    OutputBase: int;
    SourceBase: int;
    InputEncoding: Encoding;
    OutputEncoding: Encoding;
    SourceEncoding: Encoding;
    OutputFilePath: string;
    SourceFilePath: string; // file being run
  }