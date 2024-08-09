namespace dpenner1.Primell

exception PrimellInvalidSyntaxException of string

// TODO - this is probably better as an enum (if that exists in F#)
type OperationModifier = 
  | Power
  | Truncate
  | Unfold

module ParseLib =

  let rec private ParseInteger' (settings: PrimellConfiguration) (text:string) (index:int) (cumulativeValue: bigint) =
    let b = settings.SourceBase
    if index >= text.Length then 
      cumulativeValue
    else
      let c = text[index]
      let digitValue = 
        if c >= '0' && c <= '9' then 
          int c - int '0'
        elif c >= 'A' && c <= 'Z' && b > 10 then
          int c - int 'A' + 10
        elif c >= 'a' && c <= 'z' && b > 10 then
          if b <= 36 then int c - int 'A' + 10
          else int c - int 'A' + 36
        elif c = settings.Character63 && b > 62 then 62 
        elif c = settings.Character64 && b > 63 then 63
        else PrimellInvalidSyntaxException $"Invalid integer character '{c}' in: {text}" |> raise
          
      ParseInteger' settings text (index + 1) (cumulativeValue + bigint digitValue * bigint.Pow(b, text.Length - index - 1))

  let ParseInteger (settings: PrimellConfiguration) (text:string) =
    let result = 
      if settings.SourceBase = 1 then
        BigRational(text.Length, 1)
      else
        BigRational(ParseInteger' settings text 0 0I, 1) 
    result |> Rational |> PNumber

  let ParseOperationModifiers (opModText: string) =
    let rec ParseOperationModifiers' (opModText: string) (opMods: list<OperationModifier>) =
      if opModText.Length = 0 then 
        opMods
      else
        let opMod = 
          match opModText[0] with
          | '^' -> Power
          | '`' -> Truncate
          | ':' -> Unfold
          | _ as c -> PrimellInvalidSyntaxException $"Invalid operation modifier: {c}" |> raise
        ParseOperationModifiers' (opModText.Substring(1)) (opMod::opMods)

    ParseOperationModifiers' opModText []


  // you know, its just occurred to me that I have Antlr in this project already, I could use that to parse the settings...
  let rec UpdateSettings (settings: PrimellConfiguration) (args: List<string>) =
    if args.IsEmpty then 
      settings
    elif args |> List.contains "-ld" || args |> List.contains "--listell-default" then  // TODO - there's a few settings (like SourcePath) these shouldn't override
      UpdateSettings PrimellConfiguration.Listell (args |> List.where(fun arg -> arg <> "-ld" && arg <> "--listell-default"))
    elif args |> List.contains "-pd" || args |> List.contains "--primell-default" then 
      UpdateSettings PrimellConfiguration.PrimellDefault (args |> List.filter(fun arg -> arg <> "-pd" && arg <> "--primell-default"))
    else 
      match args[0].ToLowerInvariant() with
      | "-b" | "--base" ->    
        UpdateSettings { settings with InputBase = int args[1]; OutputBase = int args[1]; SourceBase = int args[1]} args.Tail.Tail  // i mean it works 
      | "-ib" | "--input-base" -> 
        UpdateSettings { settings with InputBase = int args[1] } args.Tail.Tail
      | "-ob" | "--output-base" -> 
        UpdateSettings { settings with OutputBase = int args[1] } args.Tail.Tail
      | "-sb" | "--source-base" -> 
        UpdateSettings { settings with SourceBase = int args[1] } args.Tail.Tail
      | "-c63" | "--character-63" -> 
        UpdateSettings { settings with Character63 = char args[1] } args.Tail.Tail
      | "-c64" | "--character-64" -> 
        UpdateSettings { settings with Character64 = char args[1] } args.Tail.Tail
      | "-rs" | "--restricted-source" ->   // TODO - is there a way to clean up the code copy for optional settings?
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with RestrictedSource = args[1].ToLowerInvariant().StartsWith "y" } args.Tail.Tail
        else
          UpdateSettings { settings with RestrictedSource = not settings.RestrictedSource } args.Tail
      | "-po" | "--use-prime-operators" ->
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with UsePrimeOperators = args[1].ToLowerInvariant().StartsWith "y" } args.Tail.Tail
        else
          UpdateSettings { settings with UsePrimeOperators = not settings.RestrictedSource } args.Tail
      | "-tp" | "--truth-prime" ->
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with PrimesAreTruth = args[1].ToLowerInvariant().StartsWith "y" } args.Tail.Tail
        else
          UpdateSettings { settings with PrimesAreTruth = not settings.RestrictedSource } args.Tail
      | "-ta" | "--truth-all" ->
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with RequireAllTruth = args[1].ToLowerInvariant().StartsWith "y" } args.Tail.Tail
        else
          UpdateSettings { settings with RequireAllTruth = not settings.RestrictedSource } args.Tail
      | "-sf" | "--source-filepath" ->
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with SourceFilePath = args[1] } args.Tail.Tail
        else
          UpdateSettings { settings with SourceFilePath = "" } args.Tail
      | "-of" | "--output-filepath" ->
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with OutputFilePath = args[1] } args.Tail.Tail
        else
          UpdateSettings { settings with OutputFilePath = "" } args.Tail
      | "-r" | "--rounding-mode" ->  // would have abbreviated -rm, but that's scary
        System.NotImplementedException () |> raise
      | "-p" | "--precision" ->
        System.NotImplementedException () |> raise
      | _ -> System.ArgumentException("Invalid settings argument") |> raise
