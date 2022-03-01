#include <FastLED.h>
#include <WiFi.h>

//Led
#define ledPin 2
#define ledCount 171
CRGB leds[ledCount];

//Network
char* ssidDavid = "Vodafone-113110";
char* pwDavid = "6Argynxu2LLQL67q";
char* ssidVivi = "o2-WLAN53";
char* pwVivi = "5264829999549261";
IPAddress ip(192, 168, 2, 69);
IPAddress gateway(192, 168, 2, 1);
IPAddress subnet(255, 255, 0, 0);
IPAddress DNS1(8, 8, 8, 8);
IPAddress DNS2(8, 8, 4, 4);
WiFiServer server(80);
int inDeBeningin;


//General:
#define ModeCount 4
#define Mode1 0
#define Mode2 1
#define Mode3 2
#define Mode4 3

bool loaded = false;
bool On = false;
uint8_t Mode = 0;
uint8_t Brightness = 0;
String answer = "Connected";

float SpeedVar[ModeCount] = { 0, 0, -1, -1 };
float SquishVar[ModeCount] = { -1, 0, -1, -1 };


void setup() {
	Serial.begin(115200);
	FastLED.addLeds<WS2812B, ledPin, GRB>(leds, ledCount);
	FastLED.setBrightness(Brightness);
	FastLED.show();

	checkNetwork();
	inDeBeningin = millis();
	connectNetwork(ssidVivi, pwVivi);

}



void checkNetwork() {
	Serial.println("");
	Serial.println("");
	int numSSid = WiFi.scanNetworks();
	if (numSSid == -1) {
		Serial.println("No Network detected");
	}

	for (int j = 0; j < numSSid; j++) {
		Serial.print(j);
		Serial.print(": ");
		Serial.println(WiFi.SSID(j));
	}
}
void connectNetwork(char* name, char* pass) {
	Serial.print("connecting to: ");
	Serial.println(name);
	Serial.print("using pw: ");
	Serial.println(pass);

	WiFi.config(ip, gateway, subnet, DNS1, DNS2);
	WiFi.begin(name, pass);
	while (WiFi.status() != WL_CONNECTED) {
		int End = millis();
		if ((End - inDeBeningin) > 10000) { Serial.println("Restarting\n\n"); ESP.restart(); }
		Serial.println(WiFi.status());
		delay(500);
	}
	Serial.print("WiFi Connected. IP : ");
	Serial.println(WiFi.localIP());
	server.begin();
}

void interpretLine(String request) {
	request.remove(0, 5);
	request.replace(",", ".");
	if (request.equals("Ping")) { answer = "Connected"; }
	if (request.equals("Status")) {
		if (loaded) {
			answer = "Loaded: " + (String)loaded;
			answer += "\nOn: " + (String)On;
			answer += "\nMode: " + (String)Mode;
			answer += "\nBrightness: " + (String)Brightness;
			answer += "\nSpeed: " + (String)SpeedVar[Mode];
			answer += "\nSquish: " + (String)SquishVar[Mode];
		}
		else {
			answer = "Loaded: " + (String)loaded;
		}
		Serial.println("\nLoaded: " + (String)loaded);
		Serial.println("On: " + (String)On);
		Serial.println("Mode: " + (String)Mode);
		Serial.println("Brightness" + (String)Brightness);
		Serial.println("Speed" + (String)SpeedVar[Mode]);
		Serial.println("Squish: " + (String)SquishVar[Mode]);
	}

	if (request.equals("On")) { On = true; answer = "On: " + (String)On; }
	if (request.equals("Off")) { On = false; answer = "On: " + (String)On; }

	if (request.startsWith("Mode")) {
		request.remove(0, 4);
		Mode = request.toFloat();
		answer = "Mode: " + (String)Mode;
		loaded = true;
	}
	if (request.startsWith("Brightness")) {
		request.remove(0, 10);
		Brightness = request.toFloat();
		answer = "Brightness: " + (String)Brightness;
		loaded = true;
	}
	if (request.startsWith("Speed")) {
		request.remove(0, 5);
		SpeedVar[Mode] = request.toFloat();
		answer = "Speed: " + (String)SpeedVar[Mode];
		loaded = true;
	}
	if (request.startsWith("Squish")) {
		request.remove(0, 6);
		SquishVar[Mode] = request.toFloat();
		answer = "Squish: " + (String)SquishVar[Mode];
		loaded = true;
	}
	if (request.startsWith("Load")) {
		request.remove(0, 4);

	}
	FastLED.setBrightness(Brightness);
}


void network() {
	WiFiClient client = server.available();   // listen for incoming clients

	if (client) {                             // if you get a client,
		Serial.println("New Client.");           // print a message out the serial port
		String currentLine = "";                // make a String to hold incoming data from the client
		while (client.connected()) {            // loop while the client's connected
			if (client.available()) {             // if there's bytes to read from the client,
				char c = client.read();             // read a byte, then
				Serial.write(c);                    // print it out the serial monitor
				if (c == '\n') {                    // if the byte is a newline character

				  // if the current line is blank, you got two newline characters in a row.
				  // that's the end of the client HTTP request, so send a response:
					if (currentLine.length() == 0) {
						// HTTP headers always start with a response code (e.g. HTTP/1.1 200 OK)
						// and a content-type so the client knows what's coming, then a blank line:
						client.println("HTTP/1.1 200 OK");
						client.println("Content-type:text/html");
						client.println();

						// the content of the HTTP response follows the header:


						client.print(answer);
						Serial.println(answer);


						// The HTTP response ends with another blank line:
						client.println();
						// break out of the while loop:
						break;
					}
					else {    // if you got a newline, then clear currentLine:
						currentLine = "";
					}
				}
				else if (c != '\r') {  // if you got anything else but a carriage return character,
					currentLine += c;      // add it to the end of the currentLine
				}

				interpretLine(currentLine);
			}
		}
		// close the connection:
		client.stop();
		Serial.println("Client Disconnected.");
	}
}


void hueRainbow() {
	static float hue = 0;
	static float SqshConst = (float)255 / (float)ledCount;
	for (int j = 0; j < ledCount; j++) {
		leds[j] = CHSV((uint8_t)(hue - (j * SqshConst * SquishVar[Mode2])), 255, 255);
	}
	hue += SpeedVar[Mode2];
	FastLED.show();
	delay(10);
}
void statickRainbow() {
	static float hue = 0;
	for (int i = 0; i < ledCount; i++) {
		leds[i] = CHSV((uint8_t)hue, 255, 255);
	}
	FastLED.show();
	hue += SpeedVar[Mode1];
	delay(10);
}
void SetLeds() {
	if (On) {
		switch (Mode) {
		case 1: hueRainbow();
			break;
		case 0: statickRainbow();
			break;
		}
	}
	else {
		for (int i = 0; i < ledCount; i++) {
			leds[i] = CHSV(0, 0, 0);
		}
		FastLED.show();
	}
}

void loop() {
	network();
	SetLeds();
}
