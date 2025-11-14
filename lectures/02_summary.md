# สรุป
## Onion Archtecture
เป็นสถาปัตยกรรมที่ออกแบบส่วนการทำงานต่าง ๆ ออกเป็นชั้น ๆ เพื่อ <mark>แยกตรรกะทางธุรกิจออกจากการพึ่งพาเทคโนโลยี</mark>

<p align="center">
  <img src="./images/01/1.png" alt="onion architecture layer" width="300"/>
</p>

## รายละเอียดแต่ละชั้น
### Domain Layer
เป็นหัวใจหลักของแอปพลิเคชัน จัดเก็บ 

- กฏทางธุรกิจ (Business Rule)
- การดำเนินการทางธุรกิจ (Business Logic)
- เอ็นติติ้ (Entity)
- ออบเจ็กต์ของข้อมูล (Value Object)

โดยในชั้นนี้ จะไม่มีการพึ่งพาชั้นอื่น ๆ <mark>เพื่อให้มั่นใจว่า การเปลี่ยนแปรงของเทคโนโลยี จะไม่ส่งผลกระทบต่อการดำเนินการทางธุรกิจ</mark>

### Service Layer
ทำหน้าที่ <mark>ควบคุมกระบวนการทางธุรกิจ</mark> และประสานงานการทำงานระหว่าง domain layer และ infrastructure layer

โดยในชั้นนี้จะ<mark>ไม่มีการดำเนินการเอง ที่เกี่ยวข้องกับการติดต่อกับบริการภายนอก เช่น การติดต่อกับฐานข้อมูล</mark> โดยจะสั่งให้ infrastructure layer รับผิดชอบในส่วนนี้

### Infrastructure Layer
ทำหน้าที่ <mark>จัดการเรื่องเทคโนโลยีและการติดต่อกับบริการภายนอก</mark> เช่น

- ฐานข้อมูล
- การจัดการระบบไฟล์
- การเชื่อมต่อกับ api ภายนอก
- การเก็บ log

**การพึ่งพา**

- Domain Layer
- Service Layer

### Presentation Layer
ทำหน้าที่ <mark>ติดต่อกับผู้ใช้โดยตรง</mark> เช่น

- web ui
- rest api
- comand line

โดยจะรับ input จากผู้ใช้ และส่งให้ service layer ทำการประมวลผล และนำผลลัพธ์ส่งคืนให้กับผู้ใช้

## สิ่งที่ควรรู้
### Value Object
เป็นออปเจ็กต์ที่เก็บข้อมูล ๆ หนึ่งไว้พร้อมกับกฏทางธุรกิจ โดยข้อมูลที่ถูกเก็บไว้จะอยู่ในรูปแบบ Immutable (ไม่เปลี่ยนค่าในตัวเอง) 

**ตัวอย่าง** *จำนวนเงินจะต้องมีค่ามากกว่า 0 เสมอ*

    public class Money
    {
        public decimal Amount { get; init; }

        public Money(decimal amount)
        {
            if (amount < 0)
                throw new BusinessRuleException("จำนวนเงินจะต้องมีค่ามากกว่าหรือเท่ากับ 0");

            this.Amount = amount;
        }

        public Money Add(decimal amount)
        {
            return new Money(this.amount + amount);
        }
    }

### Business Rule
เป็นข้อจำกัดหรือเงื่อนไขที่ระบบต้องปฏับิติตามเพื่อไม่ให้ผิดหลักทางธุรกิจ

**ตัวอย่าง**
- **ระบบธนาคาร** ผู้ใช้ไม่สามารถถอนเงินได้เกินยอดเงินในบัญชี
- **ระบบขายสินค้า** สินค้า 1 ชิ้นต้องมีราคามากกว่า 0 บาท
- **ระบบสั่งอาหาร** ไม่สามารถสั่งอาหารได้หลังเวลาปิดร้าน
- **ระบบบัตรสมาชิก** ผู้ใช้ต้องมีอายุอย่างน้อย 18 ปีขึ้นไป

โดย business rule ถูกแบ่งออกเป็น 2 ประเภท ได้แก่
1. **self-contained rule** เป็นชนิดที่ข้อมูลนั้น ๆ สามารถตรวจสอบเงื่อนไขได้ด้วยตัวเอง โดยไม่ต้องพึ่งพาข้อมูลจากแหล่งอื่น
2. **cross-entity rule** เป็นชนิดที่ข้อมูลนั้น ๆ ไม่สามารถตรวจสอบเงื่อไขได้ในตัวเอง จำเป็นต้องพึ่งข้อมูลจากแหล่งอื่นในการตรวจสอบ

**ตัวอย่าง** Self-contained rule

จำนวนเงินที่ระบุ จะต้องมากกว่าหรือเท่่ากับ 0
    
    // สามารถตรวจสอบได้ในตัวเอง ไม่จำเป็นต้องพึ่งข้อมูลจากแหล่งอื่น
    public class Money
    {
        public decimal Amount { get; init; }

        public Money(decimal amount)
        {
            if (amount < 0)
                throw new BusinessRuleException("จำนวนเงินจะต้องมีค่ามากกว่าหรือเท่ากับ 0");

            this.Amount = amount;
        }

        public Money Add(decimal amount)
        {
            return new Money(this.amount + amount);
        }
    }

**ตัวอย่าง** Cross-entity rule

ลูกค้าที่ถูกระงับบัญชี จะไม่สามารถซื้อสินค้าได้

    // ข้อมูลของการสั่งซื้อสินค้า จะไม่รู้ว่าผู้ใช้คนนี้ถูกระงับบัญชีไปหรือยัง จำเป็นต้องพึ่งพาข้อมูลจากแหล่งอื่นเพื่อเข้าถึงสถานะของผู้ใช้
    // ในที่นี้ ข้อมูลจากแหล่งอื่นคือ customer
    public class OrderDomainService
    {
        public Order CreateOrder(Customer customer, List<OrderItem> items)
        {
            if (!customer.IsActive)
                throw new BusinessRuleException("ลูกค้าถูกระงับบัญชี ไม่สามารถสั่งซื้อได้");
            ...
        }
    }

### Business Logic
เป็นกระบวนการหลักที่ระบบต้องทำเพื่อให้บรรลุเป้าหมาย

**ตัวอย่าง**
- **ระบบธนาคาร** คำนวนดอกเบี้ยเงินฝาก, ย้ายเงินระหว่างบัญชี
- **ระบบขายสินค้า** คำนวนราคารวม, ลดราคา, สร้างใบเสนอราคา
- **ระบบสั่งอาหาร** รวมเมนูที่สั่ง, คิดค่าส่ง, สร้างใบเสนอราคา
- **ระบบสมัครสมาชิก** สร้างบัญชีใหม่, ตรวบสอบอีเมลซ้ำ, ยืนยันตัวตน

### ความแตกต่างระหว่าง service ใน domain layer และ application layer
- **service ใน domain layer** ทำหน้าที่ดำเนินการตรรกะทางธุรกิจเป็นหลัก แต่ไม่มีการเชื่อมต่อกับ infrastructure (ยกเว้น repository)
- **service ใน application layer** ทำหน้าที่เชื่อมต่อ domain service และ infrastructure เข้าด้วยกัน

### เกี่ยวกับตำแหน่งของ interface repository
ตำแหน่งของ interface repository จะแตกต่างกันออกไปตามแนวคิดที่โปรเจคนั้นใช้

- หาก interface repository อยู่ใน domain layer แสดงว่าใช้แนวคิด domain driven design
- หาก interface repository อยู่ใน service layer แสดงว่าใช้แนวคิด clean architecture/cqrs

