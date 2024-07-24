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
    RestrictedSource: bool;
    UsePrimeOperators: bool;
    TruthDefinition: TruthDefinition;
    InputBase: int;
    OutputBase: int;
    SourceBase: int;
    InputEncoding: Encoding;
    OutputEncoding: Encoding;
    SourceEncoding: Encoding;
    SourceFilePath: string; // file being run
    OutputFilePath: string;
  }
  static member PrimellDefault = 
    {
      RestrictedSource = true
      UsePrimeOperators = true
      TruthDefinition = { PrimesAreTruth = true; EmptyIsTruth = false; RequireAllTruth = true }
      InputBase = 10
      OutputBase = 10
      SourceBase = 10
      // The encoding stuff might change (eg. surely a terminal's encoding should override)
      InputEncoding = Encoding.UTF8
      OutputEncoding = Encoding.UTF8
      SourceEncoding = Encoding.UTF8
      SourceFilePath = ""
      OutputFilePath = ""
    }
  
  // Listell is the name of the "serious" version of the language without the prime shenanigans
  static member Listell = 
    {
      RestrictedSource = false
      UsePrimeOperators = false
      TruthDefinition = { PrimesAreTruth = false; EmptyIsTruth = false; RequireAllTruth = false }
      InputBase = 10
      OutputBase = 10
      SourceBase = 10
      // The encoding stuff might change (eg. surely a terminal's encoding should override)
      InputEncoding = Encoding.UTF8
      OutputEncoding = Encoding.UTF8
      SourceEncoding = Encoding.UTF8
      OutputFilePath = ""
      SourceFilePath = ""                           
    }
