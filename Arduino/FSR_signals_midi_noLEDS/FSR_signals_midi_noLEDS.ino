#include <FastLED.h>
const int drumCount = 6;
#define LED_PIN     0
#define COLOR_ORDER GRB
#define CHIPSET     WS2811
#define NUM_LEDS    20
#define BRIGHTNESS  200
#define FRAMES_PER_SECOND 30
// COOLING: How much does the air cool as it rises?
// Less cooling = taller flames.  More cooling = shorter flames.
// Default 55, suggested range 20-100 
#define COOLING  30
// SPARKING: What chance (out of 255) is there that a new spark will be lit?
// Higher chance = more roaring fire.  Lower chance = more flickery fire.
// Default 120, suggested range 50-200.
#define SPARKING 200
bool gReverseDirection = false;
CRGB leds[drumCount][NUM_LEDS];

const int threshold = 920;
const int peakDelay =35;
const int bounceDelay=30;
const int channel = 10; //MIDI channel
int notes[drumCount];
bool hits[drumCount];
bool bounceReset[drumCount];
int vals[drumCount];
bool sent[drumCount];
int maxVals[drumCount];
unsigned long delayStart[drumCount];
unsigned long bounceStart[drumCount];
int hitCount =0;

CRGBPalette16 gPal[3];


void setup() {
   FastLED.addLeds<CHIPSET, 0, COLOR_ORDER>(leds[0], NUM_LEDS).setCorrection( TypicalLEDStrip );
   FastLED.addLeds<CHIPSET, 1, COLOR_ORDER>(leds[1], NUM_LEDS).setCorrection( TypicalLEDStrip );
   FastLED.addLeds<CHIPSET, 2, COLOR_ORDER>(leds[2], NUM_LEDS).setCorrection( TypicalLEDStrip );
   FastLED.addLeds<CHIPSET, 3, COLOR_ORDER>(leds[3], NUM_LEDS).setCorrection( TypicalLEDStrip );
   FastLED.addLeds<CHIPSET, 4, COLOR_ORDER>(leds[4], NUM_LEDS).setCorrection( TypicalLEDStrip );
   FastLED.addLeds<CHIPSET, 5, COLOR_ORDER>(leds[5], NUM_LEDS).setCorrection( TypicalLEDStrip );
   FastLED.setBrightness( BRIGHTNESS );

  gPal[0] = CRGBPalette16( CRGB::Black, CRGB::Red, CRGB::Orange, CRGB::White);
  gPal[1] = CRGBPalette16( CRGB::Black, CRGB::Blue, CRGB::Aqua,  CRGB::White);
  gPal[2] = CRGBPalette16( CRGB::Black, CRGB::Green, CRGB::Yellow, CRGB::White);

  notes[0] = 21;
  notes[1] = 22;
  notes[2] = 23;
  notes[3] = 24;
  notes[4] = 25;
  notes[5] = 26;

  // put your setup code here, to run once:
  for (int i=0;i<drumCount;i++) {
    hits[i] = false;
    bounceReset[i]=false;
    maxVals[i] = 0;
    sent[i] = false;
  }
  Serial.begin(115200);

  Serial.println("Setup complete");
  
}


int getHitStrength(int hitVal) {
  if (hitVal > 920) {
    return 3;
  } else if (hitVal > 750) {
    return 2;
  } else {
    return 1;
  }
}

void loop() {
 
  //Fire2012WithPalette(); // run simulation frame, using palette colors
  //FastLED.show(); // display this frame
  //FastLED.delay(1000 / FRAMES_PER_SECOND);
  
  for (auto i =0;i<drumCount;i++) {
    
    vals[i] = analogRead(i);
    if (vals[i] > threshold && hits[i] == false) {
      hitLeds(i); //trigger leds to flash colour when hit
      maxVals[i]=vals[i];
      hitCount++;
      hits[i] = true;
      delayStart[i] = millis();
      //Serial.println("hit");
    } else if (hits[i]) {
      
      if (vals[i] > maxVals[i] && !sent[i]) maxVals[i] = vals[i];

      if ( millis()-delayStart[i] >= peakDelay && !sent[i]) {
        sent[i] = true; //signal that hit message is sent to computer
        usbMIDI.sendNoteOn(notes[i], vals[i], channel);
        maxVals[i]=0; //reset max val for next hit
        
      }
      if (sent[i] && vals[i] < threshold && bounceReset[i]==false) { //once signal sent and signal at rest and bounce delay hasn't started
        //Serial.println("bounce start");
        bounceStart[i]= millis(); //start bounce delay
        bounceReset[i]=true;
      }

      if (bounceReset[i] && millis()-bounceStart[i] >= bounceDelay && vals[i] < threshold) { //once bounce delay triggered and delay complete and sensor at rest
        //Serial.println("reset");
        hits[i]=false; //reset drum hit to allow another
        sent[i]=false; //reset sent signal to allow another signal to be sent
        bounceReset[i]=false; //reset bounce delay signal for next hit
        maxVals[i]=0;
      }
    }
  }
  
}
void hitLeds(int drum) {
  CRGB colour;
  if (drum >=4) {
    colour = CRGB::Blue;
  }else if (drum >= 2){
    colour = CRGB::Red;
  }else {
    colour = CRGB::Red;
  }
  for (int i=0;i<NUM_LEDS;i++) {
    leds[drum][i] = CRGB::Red;
  }
  FastLED.show(); // display this frame
  FastLED.delay(125);
  
}


void Fire2012WithPalette()
{
// Array of temperature readings at each simulation cell
  for (int player=0;player<3;player++) {
    static uint8_t heat[NUM_LEDS];

  // Step 1.  Cool down every cell a little
    for( int i = 0; i < NUM_LEDS; i++) {
      heat[i] = qsub8( heat[i],  random8(0, ((COOLING * 10) / NUM_LEDS) + 2));
    }
  
    // Step 2.  Heat from each cell drifts 'up' and diffuses a little
    for( int k= NUM_LEDS - 1; k >= 2; k--) {
      heat[k] = (heat[k - 1] + heat[k - 2] + heat[k - 2] ) / 3;
    }
    
    // Step 3.  Randomly ignite new 'sparks' of heat near the bottom
    if( random8() < SPARKING ) {
      int y = random8(7);
      heat[y] = qadd8( heat[y], random8(160,255) );
    }

    // Step 4.  Map from heat cells to LED colors
    for( int j = 0; j < NUM_LEDS; j++) {
      // Scale the heat value from 0-255 down to 0-240
      // for best results with color palettes.
      uint8_t colorindex = scale8( heat[j], 240);
      CRGB color = ColorFromPalette( gPal[player], colorindex);
      int pixelnumber;
      if( gReverseDirection ) {
        pixelnumber = (NUM_LEDS-1) - j;
      } else {
        pixelnumber = j;
      }
      leds[player*2][pixelnumber] = color;
      leds[(player*2)+1][pixelnumber] = color;
    }
  }
  
}
