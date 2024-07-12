namespace dpenner1.PrimellF

module ParseLib =

  let rec private ParseInteger' (text:string) (b:int) (index:int) (cumulativeValue: bigint) =
    if index >= text.Length then 
      cumulativeValue
    else
      let c = text[index]
      let digitValue = 
        if c >= '0' && c <= '9' then 
          int c - int '0'
        elif c >= 'A' && c <= 'Z' then
          int c - int 'A' + 10
        elif c >= 'a' && c <= 'z' then
          if b <= 36 then int c - int 'A' + 10
          else int c - int 'A' + 36
        elif c = 'Þ' then 62
        elif c = 'þ' then 63
        else failwith "invalid character"
          
      ParseInteger' text b (index + 1) (cumulativeValue + bigint digitValue * bigint.Pow(b, text.Length - index - 1))

  let ParseInteger (text:string) (``base``:int) =
    let b = ``base``
    if b = 1 then
      BigRational(text.Length, 1) |> Number 
    else
      BigRational(ParseInteger' text b 0 0I, 1) |> Number