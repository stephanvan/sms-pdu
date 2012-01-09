using System;
using System.Collections.Generic;
using System.Text;

namespace SMSPDULib
{
	public class GSM0338Charset
	{
		private const byte ESCAPE_BYTE = 0x1B;
		private static readonly char[] _gsmToUTF = new[]
		{
			'@', '£', '$', '¥', 'è', 'é', 'ù', 'ì', 'ò', 'Ç', (char)0x0A, 'Ø', 'ø', (char)0x0D, 'Å', 'å',
			'Δ', '_', 'Φ', 'Γ', 'Λ', 'Ω', 'Π', 'Ψ', 'Σ', 'Θ', 'Ξ', (char)0xA0, 'Æ', 'æ', 'ß', 'É',
			' ', '!', '"', '#', '¤', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',
			'¡', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
			'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Ä', 'Ö', 'Ñ', 'Ü', '§',
			'¿', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
			'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ä', 'ö', 'ñ', 'ü', 'à'
		};

		private static readonly char[] _gsmToUTFExtended = new[]
		{
			'?', '?', '?', '?', '?', '?', '?', '?', '?', '?', (char)0x0C, '?', '?', '?', '?', '?', 
			'?', '?', '?', '?', '^', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', 
			'?', '?', '?', '?', '?', '?', '?', '?', '{', '}', '?', '?', '?', '?', '?', '\\', 
			'?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '[', '~', ']', '?', 
			'|', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', 
			'?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', 
			'?', '?', '?', '?', '?', '€', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', 
			'?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?', '?'
		};

		private static SortedList<char, byte[]> _utfToGSM = new SortedList<char, byte[]>();

		static GSM0338Charset()
		{
			for( int i = 0; i < _gsmToUTF.Length; i++ )
			{
				_utfToGSM.Add( _gsmToUTF[i], new[] { (byte)i } );
			}
			for( int i = 0; i < _gsmToUTFExtended.Length; i++ )
			{
				if( _gsmToUTFExtended[i] != '?' )
				{
					_utfToGSM.Add( _gsmToUTFExtended[i], new[] { ESCAPE_BYTE, (byte)i } );
				}
			}
		}

		public static byte[] UTFToGSM( string str )
		{
			List<byte> bytes = new List<byte>();

			for( int i = 0; i < str.Length; i++ )
			{
				if( _utfToGSM.ContainsKey( str[i] ) )
					bytes.AddRange( _utfToGSM[str[i]] );
				else
					bytes.AddRange( _utfToGSM['?'] ); //unknown character
			}

			return bytes.ToArray();
		}

		public static string GSMToUTF( byte[] gsmbytes )
		{
			List<char> chars = new List<char>();

			bool isExtendedChar = false;
			for( int i = 0; i < gsmbytes.Length; i++ )
			{
				char c = _gsmToUTF[gsmbytes[i]];
				if( c == _gsmToUTF[ESCAPE_BYTE] )
				{
					if( isExtendedChar ) //second ESC in a row
						throw new GSMEncodingException("Invalid Escape sequence");
					isExtendedChar = true;
					continue;
				}
				if( isExtendedChar )
				{
					c = _gsmToUTFExtended[gsmbytes[i]];
					isExtendedChar = false;
				}
				chars.Add( c );
			}

			return new String( chars.ToArray() );
		}

        /// <summary>
        /// Return a GSM 03.38 equivalent string of input string
        /// [ummarbhutta@gmail.com]
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Returns a GSM 0338 equivalent string</returns>
        public static string ConvertToGSM0338String(string str)
        {
            byte[] buff = UTFToGSM(str);
            return Encoding.ASCII.GetString(buff);
        }
	}
}
