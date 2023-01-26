
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}
void loop() {
  // put your main code here, to run repeatedly:
  int val1 = analogRead(0);
  int val2 = analogRead(2);
  int val3 = analogRead(3);

  if (val1 > 10) {
    Serial.println(val1);
  }
}
