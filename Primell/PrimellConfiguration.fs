namespace dpenner1.Primell

open System.Text

type PrimellConfiguration =
  {
    RestrictedSource: bool;
    UsePrimeOperators: bool;
    PrimesAreTruth: bool;  // If true, only primes are true, if false, non-zero., non-NaN is true
    RequireAllTruth: bool;
    InputBase: int;
    OutputBase: int;
    SourceBase: int;
    Character63: char;
    Character64: char;
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
      PrimesAreTruth = true
      RequireAllTruth = true
      InputBase = 10
      OutputBase = 10
      SourceBase = 10
      Character63 = 'Þ'
      Character64 = 'þ'
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
      PrimesAreTruth = false
      RequireAllTruth = false
      InputBase = 10
      OutputBase = 10
      SourceBase = 10
      Character63 = 'Þ'
      Character64 = 'þ'
      // The encoding stuff might change (eg. surely a terminal's encoding should override)
      InputEncoding = Encoding.UTF8
      OutputEncoding = Encoding.UTF8
      SourceEncoding = Encoding.UTF8
      OutputFilePath = ""
      SourceFilePath = ""                           
    }
