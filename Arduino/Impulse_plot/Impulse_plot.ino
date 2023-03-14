
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}
void loop() {
  // put your main code here, to run repeatedly:
  int val1 = analogRead(0);
  int val2 = analogRead(1);
  int val3 = analogRead(2);  
  int val4 = analogRead(3);  
  
  if (val1 > 200) {
    Serial.println(val1);
  }
  if (val2 > 200) {
    Serial.println(val2);
  }
  if (val3 > 200) {
    Serial.println(val3);
  }
    if (val4 > 200) {
    Serial.println(val4);
  }

}
