public enum LetterStatus {
    Unknown,
    Unused,
    WrongPosition,
    CorrectPosition
}

public static class LetterStatusExtensions {
    public static LetterStatus Parse(char ToParse){
        switch(ToParse){
            case 'b':
                return LetterStatus.Unused;
            case 'g':
                return LetterStatus.CorrectPosition;
            case 'y':
                return LetterStatus.WrongPosition;
            case 'u':
                return LetterStatus.Unknown;
            default:
                throw new InvalidDataException("Provided letter status is invalid");
        }
    }
}