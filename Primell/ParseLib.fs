namespace dpenner1.Primell

exception PrimellInvalidSyntaxException of string

// TODO - this is probably better as an enum (if that exists in F#)
type OperationModifier = 
  | Power
  | Cut

module ParseLib =

  let rec private ParseInteger' (text:string) (b:int) (index:int) (cumulativeValue: bigint) =
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
        elif c = 'Þ' && b > 62 then 62 
        elif c = 'þ' && b > 63 then 63
        else PrimellInvalidSyntaxException $"Invalid integer character '{c}' in: {text}" |> raise
          
      ParseInteger' text b (index + 1) (cumulativeValue + bigint digitValue * bigint.Pow(b, text.Length - index - 1))

  let ParseInteger (text:string) (``base``:int) =
    let b = ``base``
    let result = 
      if b = 1 then
        BigRational(text.Length, 1)
      else
        BigRational(ParseInteger' text b 0 0I, 1) 
    result |> Rational |> PNumber

  let rec private ParseOperationModifiers' (opModText: string) (opMods: list<OperationModifier>) =
    if opModText.Length = 0 then 
      opMods
    else
      let opMod = 
        match opModText[0] with
        | '^' -> Power
        | '`' -> Cut
        | _ as c -> PrimellInvalidSyntaxException $"Invalid operation modifier: {c}" |> raise

      ParseOperationModifiers' (opModText.Substring(1)) (opMod::opMods)


  let ParseOperationModifiers (opModText: string) =
    ParseOperationModifiers' opModText []


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
      | "-rs" | "--restricted-source" ->   // TODO - is there a way to clean up the code copy for optional settings?
        if args.Length > 1 && not <| args[1].StartsWith "-" then
          UpdateSettings { settings with RestrictedSource = args[1].ToLowerInvariant().StartsWith "y" } args.Tail.Tail
        else
          UpdateSettings { settings with RestrictedSource = not settings.RestrictedSource } args.Tail
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
